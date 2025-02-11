using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public class Member
{
    public long UserId { get; set; }
    public virtual User User { get; set; }
    
    public long RoomId { get; set; }
    public virtual Room Room { get; set; }

    public Role Role { get; set; }
}