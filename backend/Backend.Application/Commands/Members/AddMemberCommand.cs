using Backend.Domain.Abstractions.Commands;
using Backend.Domain.Entities;
using Backend.Domain.Enums;

namespace Backend.Application.Commands.Members;

public class AddMemberCommand : ICommand<Member>
{
    public long? UserId { get; set; }
    public long? RoomId { get; set; }
    public Role? Role { get; set; }
}