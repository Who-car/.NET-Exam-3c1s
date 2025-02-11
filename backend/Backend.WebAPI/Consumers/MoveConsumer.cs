using Backend.Application.Common.Strings;
using Backend.Data;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.WebAPI.Common.Models;
using Backend.WebAPI.Hubs;
using Backend.WebAPI.Hubs.Clients;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.WebAPI.Consumers;

public class MoveConsumer(
    PostgresDbContext postgres,
    IHubContext<GameHub, IGameClient> hubContext,
    IMemoryCache cache
    ) : IConsumer<MoveModel>
{
    private List<RoomModel> Rooms => cache.Get<List<RoomModel>>("rooms")!;
    
    public async Task Consume(ConsumeContext<MoveModel> context)
    {
        var move = context.Message;

        var room = Rooms.FirstOrDefault(r =>
            r.Members.FirstOrDefault(m => m.RoomId == move.RoomId && m.UserId == move.UserId) != null);

        if (room is null)
            throw new ArgumentException(Errors.Room.NotFound);

        if (room.Moves.Count == 0)
        {
            room.Moves.Add(move);
            foreach (var roomMember in room.Members)
            {
                var connection = cache.Get<string>(roomMember.UserId);
                if (connection is null or "") continue;
                hubContext.Clients.Client(connection).OnMoveMade(move.Username);
            }
        }
        else if (room.Moves.Count > 0)
        {
            var opMove = room.Moves[0];
            var round = new Round
            {
                RoomId = move.RoomId,
                Moves = [
                    new Move
                    {
                        RoomId = move.RoomId,
                        UserId = move.UserId,
                        Value = move.Value,
                    }, new Move
                    {
                        RoomId = opMove.RoomId,
                        UserId = opMove.UserId,
                        Value = opMove.Value
                    }
                ]
            };
            room.Moves.Clear();
            string winnerUsername = "";
            if (opMove.Value == move.Value) { }
            else if (opMove.Value == MoveType.Paper && move.Value == MoveType.Rock
                     || opMove.Value == MoveType.Scissors && move.Value == MoveType.Paper
                     || opMove.Value == MoveType.Rock && move.Value == MoveType.Scissors)
            {
                round.WinnerUserId = opMove.UserId;
                winnerUsername = opMove.Username;
            }
            else
            {
                round.WinnerUserId = move.UserId;
                winnerUsername = move.Username;
            }

            await postgres.Rounds.AddAsync(round);
            await postgres.SaveChangesAsync();

            List<MoveModel> moves = [move, opMove];
            var dict = moves.ToDictionary(m => m.Username, m => m.Value.ToString());
            foreach (var member in room.Members)
            {
                var conn = cache.Get<string>(member.UserId);
                if (conn is null) continue;
                hubContext.Clients.Client(conn).OnWinnerCalculated(winnerUsername, dict);
            }
        }
    }
}