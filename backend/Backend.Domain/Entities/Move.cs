using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public class Move
{
    public long RoomId { get; set; }
    public Room Room { get; set; }
    
    public long UserId { get; set; }
    public User User { get; set; }
    
    public long RoundId { get; set; }
    public Round Round { get; set; }
    
    public MoveType Value { get; set; }
}