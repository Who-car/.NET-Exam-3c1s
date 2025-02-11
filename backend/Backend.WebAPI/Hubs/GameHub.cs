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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.WebAPI.Hubs;

[Authorize]
public class GameHub(
    IMemoryCache cache,
    IQueryDispatcher queryDispatcher,
    ICommandDispatcher commandDispatcher
    ) : Hub<IGameClient>
{
    private static readonly List<RoomModel> Rooms = [];
    
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
        
        cache.Set(connId, user);
        cache.Set(user.Id, connId);
        
        await base.OnConnectedAsync();
    }

    public async Task<JoinRoomResponseDto> JoinRoom(long roomId)
    {
        var connId = Context.ConnectionId;
        var user = cache.Get<User>(connId);
        
        if (user is null)
            throw new ArgumentException(Errors.Authentication.NotAuthorized);
        
        var room = Rooms.SingleOrDefault(r => r.Id == roomId);
        if (room is null)
        {
            var getRoom = new GetRoom {Id = roomId};
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
                Members = [new MemberModel(member)],
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
            if (room.Members.All(m => m.UserId != user.Id))
                room.Members.Add(new MemberModel(member));
        }
        return new JoinRoomResponseDto {JoinGame = room.Members.Count(m => m.Role == Role.Player) < 2};
    }

    public async Task<JoinGameResponseDto> JoinGame(long roomId)
    {
        var connId = Context.ConnectionId;
        var user = cache.Get<User>(connId);
        
        var room = Rooms.SingleOrDefault(r => r.Id == roomId);
        if (room is null)
            throw new InvalidOperationException();

        return new JoinGameResponseDto();
    }

    public async Task MakeMove(string value)
    {
        
    }

    public async Task LeaveGame()
    {
        
    }

    public async Task LeaveRoom()
    {
        
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var connId = Context.ConnectionId;
        var userId = cache.Get(connId) ?? "";
        cache.Remove(userId);
        cache.Remove(connId);
        
        await base.OnConnectedAsync();
    }
}