using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public class Room : IEntity
{
    public long Id { get; set; }
    
    public long MaxRating { get; set; }
    
    public DateTime CreationDate { get; set; }
    public RoomStatus Status { get; set; }

    public List<Member> Members { get; set; } = [];
    public List<Round> Rounds { get; set; } = [];
    public List<Move> Moves { get; set; } = [];
}