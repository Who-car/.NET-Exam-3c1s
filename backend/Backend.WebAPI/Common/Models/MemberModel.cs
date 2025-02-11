using Backend.Domain.Entities;
using Backend.Domain.Enums;

namespace Backend.WebAPI.Common.Models;

public class MemberModel
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public long RoomId { get; set; }
    public Role Role { get; set; }

    public MemberModel() { }
    
    public MemberModel(Member m)
    {
        UserId = m.UserId;
        Username = m.User.Username;
        RoomId = m.RoomId;
        Role = m.Role;
    }
}