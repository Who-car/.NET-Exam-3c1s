using Backend.Application.Common.Strings;
using Backend.Data;
using Backend.Domain.Abstractions.Commands;
using Backend.Domain.Entities;
using Backend.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Application.Commands.Members;

public class ChangeMemberRoleCommandHandler(
    PostgresDbContext postgres
    ) : ICommandHandler<ChangeMemberRoleCommand, Member>
{
    public async Task<Member> HandleAsync(ChangeMemberRoleCommand command)
    {
        var member = await postgres.Members
            .Where(m => m.UserId == command.UserId)
            .ToListAsync();
        
        if (member is null)
            throw new EntityNotFoundException(Errors.Member.NotFound);
        
        if (command.Role is not null)
            member.ForEach(m => m.Role = command.Role.Value);
        
        await postgres.SaveChangesAsync();
        return member[0];
    }
}