using System.Security.Claims;
using Backend.Application.Commands.Members;
using Backend.Application.Common.Strings;
using Backend.Data.QueryHandlers.Rooms;
using Backend.Data.QueryHandlers.Users;
using Backend.Domain.Abstractions.Commands;
using Backend.Domain.Abstractions.Queries;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.Exceptions;
using Backend.WebAPI.Common.Dtos;
using Backend.WebAPI.Common.Models;
using Backend.WebAPI.Hubs.Clients;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.WebAPI.Hubs;

[Authorize]
public class GameHub(
    IMemoryCache cache,
    IQueryDispatcher queryDispatcher,
    ICommandDispatcher commandDispatcher,
    IBus bus
    ) : Hub<IGameClient>
{
    private List<RoomModel> Rooms => cache.Get<List<RoomModel>>("rooms")!;

    public override async Task OnConnectedAsync()
    {
        var connId = Context.ConnectionId;
        var userIdClaim = Context.User?.FindFirstValue(CustomClaimTypes.UserId);
        if (userIdClaim is null or "" || !long.TryParse(userIdClaim, out var userId))
            throw new ArgumentException(Errors.Authentication.NotAuthorized);
        
        var query = new GetUser {Id = userId};
        var user = await queryDispatcher.DispatchAsync<GetUser, User>(query);
        if (user is null) 
            throw new ArgumentException(Errors.Authentication.NotAuthorized);
        
        cache.Set(connId, new UserModel(user));
        cache.Set(user.Id, connId);
        
        var rooms = cache.Get<List<RoomModel>>("rooms");
        if (rooms is null) 
            cache.Set("rooms", new List<RoomModel>());
        
        await base.OnConnectedAsync();
    }

    public async Task<JoinRoomResponseDto> JoinRoom(long roomId)
    {
        try
        {
            var connId = Context.ConnectionId;
            var user = cache.Get<UserModel>(connId);

            if (user is null)
                throw new ArgumentException(Errors.Authentication.NotAuthorized);

            var room = Rooms.SingleOrDefault(r => r.Id == roomId);
            if (room is null)
            {
                var getRoom = new GetRoom { Id = roomId };
                var dbRoom = await queryDispatcher.DispatchAsync<GetRoom, Room>(getRoom);
                if (dbRoom is null)
                    throw new EntityNotFoundException(Errors.Room.NotFound);

                var command = new AddMemberCommand
                {
                    UserId = user.Id,
                    RoomId = dbRoom.Id,
                    Role = Role.Spectator
                };
                var member = await commandDispatcher.DispatchAsync<AddMemberCommand, Member>(command);

                room = new RoomModel
                {
                    Id = roomId,
                    LastRoundDateTime = DateTime.MinValue.ToUniversalTime(),
                    Members = [new MemberModel(member, user)],
                    Moves = []
                };
                Rooms.Add(room);
            }
            else
            {
                var command = new AddMemberCommand
                {
                    UserId = user.Id,
                    RoomId = room.Id,
                    Role = Role.Spectator
                };
                var member = await commandDispatcher.DispatchAsync<AddMemberCommand, Member>(command);
            }

            return new JoinRoomResponseDto { JoinGame = room.Members.Count(m => m.Role == Role.Player) < 2 };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<JoinGameResponseDto> JoinGame(long roomId)
    {
        var connId = Context.ConnectionId;
        var user = cache.Get<UserModel>(connId);
        
        if (user is null)
            throw new ArgumentException(Errors.Authentication.NotAuthorized);
        
        var room = Rooms.SingleOrDefault(r => r.Id == roomId);
        if (room is null)
            throw new EntityNotFoundException(Errors.Room.NotFound);

        var member = room.Members.FirstOrDefault(u => u.UserId == user.Id);
        if (member is null)
            throw new InvalidOperationException(Errors.Room.NotAParticipant);

        if (member.Role == Role.Player)
            return new JoinGameResponseDto {IsSuccess = true};
        
        member.Role = Role.Player;
        var changeRole = new ChangeMemberRoleCommand
        {
            UserId = user.Id,
            Role = Role.Player
        };
        await commandDispatcher.DispatchAsync<ChangeMemberRoleCommand, Member>(changeRole);
        
        if (room.Members.All(m => m.UserId != user.Id))
        {
            foreach (var m in room.Members)
            {
                var conn = cache.Get<string>(m.UserId);
                if (conn is null) continue;
                Clients.Client(conn).OnUserConnected(user.Username);
            }
        }

        return new JoinGameResponseDto {IsSuccess = true};
    }

    public async Task MakeMove(string value)
    {
        if (value is null or "" || !Enum.TryParse<MoveType>(value, out var moveType))
            throw new ArgumentException(Errors.Game.WrongMove);
        
        var connId = Context.ConnectionId;
        var user = cache.Get<UserModel>(connId);
        
        if (user is null)
            throw new ArgumentException(Errors.Authentication.NotAuthorized);
        
        var room = Rooms.FirstOrDefault(r => r.Members.Any(m => m.UserId == user.Id));
        if (room is null)
            throw new InvalidOperationException(Errors.Room.NotFound);

        if ((DateTime.Now - room.LastRoundDateTime).TotalSeconds < 10)
            throw new InvalidOperationException(Errors.Game.TooEarly);
        
        var m = new MoveModel
        {
            RoomId = room.Id,
            UserId = user.Id,
            Username = user.Username,
            Value = moveType
        };
        await bus.Publish(m);
    }

    public async Task LeaveGame()
    {
        var connId = Context.ConnectionId;
        var user = cache.Get<UserModel>(connId);
        
        if (user is null)
            throw new ArgumentException(Errors.Authentication.NotAuthorized);
        
        var rooms = Rooms.Where(r => r.Members.Any(m => m.UserId == user.Id)).ToList();
        if (rooms.Count == 0) return;

        foreach (var otherMembers in rooms.SelectMany(r => r.Members.Where(m => m.UserId != user.Id).ToList()))
        {
            var conn = cache.Get<string>(otherMembers.UserId);
            if (conn is null) continue;
            Clients.Client(conn).OnUserDisconnected(user.Username);
        }
        
        var members = rooms.SelectMany(r => r.Members.Where(u => u.UserId == user.Id)).ToList();
        if (members.Count == 0) return;
        
        members.ForEach(m => m.Role = Role.Spectator);
        var changeRole = new ChangeMemberRoleCommand
        {
            UserId = user.Id,
            Role = Role.Spectator
        };
        await commandDispatcher.DispatchAsync<ChangeMemberRoleCommand, Member>(changeRole);
    }

    public Task LeaveRoom()
    {
        var connId = Context.ConnectionId;
        var user = cache.Get<User>(connId);
        
        if (user is null)
            throw new ArgumentException(Errors.Authentication.NotAuthorized);
        
        var rooms = Rooms.Where(r => r.Members.Any(m => m.UserId == user.Id)).ToList();
        if (rooms.Count == 0) return Task.CompletedTask;

        var members = rooms.SelectMany(r => r.Members.Where(u => u.UserId == user.Id)).ToList();
        if (members.Count == 0) return Task.CompletedTask;

        rooms.ForEach(r => r.Members.RemoveAll(m => m.UserId == user.Id));
        return Task.CompletedTask;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connId = Context.ConnectionId;
        var userIdCached = cache.Get(connId) ?? "";
        cache.Remove(userIdCached);
        cache.Remove(connId);

        if (long.TryParse(userIdCached.ToString(), out var userId))
        {
            Rooms.ForEach(r => r.Members.RemoveAll(m => m.UserId == userId));
            var command = new ChangeMemberRoleCommand
            {
                UserId = userId,
                Role = Role.Spectator
            };
            _ = await commandDispatcher.DispatchAsync<ChangeMemberRoleCommand, Member>(command);
        }
        
        await base.OnConnectedAsync();
    }
}