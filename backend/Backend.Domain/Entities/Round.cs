namespace Backend.Domain.Entities;

public class Round : IEntity
{
    public long Id { get; set; }
    
    public long WinnerUserId { get; set; }
    public virtual User WinnerUser { get; set; }
    
    public long RoomId { get; set; }
    public virtual Room Room { get; set; }
    
    public virtual List<Move> Moves { get; set; }
}