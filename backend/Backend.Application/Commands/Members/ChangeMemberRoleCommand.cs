using Backend.Domain.Abstractions.Commands;
using Backend.Domain.Entities;
using Backend.Domain.Enums;

namespace Backend.Application.Commands.Members;

public class ChangeMemberRoleCommand : ICommand<Member>
{
    public long? UserId { get; set; }
    public Role? Role { get; set; }
}