using Backend.Domain.Entities;
using Backend.Domain.Enums;

namespace Backend.WebAPI.Common.Models;

public class MoveModel
{
    public long RoomId { get; set; }
    public long UserId { get; set; }
    public string Username { get; set; }
    public long RoundId { get; set; }
    public MoveType Value { get; set; }

    public MoveModel() { }

    public MoveModel(Move m)
    {
        RoomId = m.RoomId;
        UserId = m.UserId;
        Username = m.User.Username;
        RoundId = m.RoundId;
        Value = m.Value;
    }
}