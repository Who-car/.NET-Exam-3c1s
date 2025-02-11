using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public class Move
{
    public long RoomId { get; set; }
    public virtual Room Room { get; set; }
    
    public long UserId { get; set; }
    public virtual User User { get; set; }
    
    public long RoundId { get; set; }
    public virtual Round Round { get; set; }
    
    public MoveType Value { get; set; }
}