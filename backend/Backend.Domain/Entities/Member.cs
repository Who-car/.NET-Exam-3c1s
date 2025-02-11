using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public class Member
{
    public long UserId { get; set; }
    public User User { get; set; }
    
    public long RoomId { get; set; }
    public Room Room { get; set; }

    public Role Role { get; set; }
}