namespace Backend.Domain.Entities;

public class Round : IEntity
{
    public long Id { get; set; }
    
    public long WinnerUserId { get; set; }
    public User WinnerUser { get; set; }
    
    public long RoomId { get; set; }
    public Room Room { get; set; }
    
    public List<Move> Moves { get; set; }
}