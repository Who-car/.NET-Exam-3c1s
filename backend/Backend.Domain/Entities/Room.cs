using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public class Room : IEntity
{
    public long Id { get; set; }
    
    public long MaxRating { get; set; }
    
    public DateTime CreationDate { get; set; }
    public RoomStatus Status { get; set; }

    public virtual List<Member> Members { get; set; } = [];
    public virtual List<Round> Rounds { get; set; } = [];
    public virtual List<Move> Moves { get; set; } = [];
}