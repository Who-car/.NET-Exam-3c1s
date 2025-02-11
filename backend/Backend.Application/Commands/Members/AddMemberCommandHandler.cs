using Backend.Application.Common.Strings;
using Backend.Data;
using Backend.Domain.Abstractions.Commands;
using Backend.Domain.Entities;
using Backend.Domain.Enums;

namespace Backend.Application.Commands.Members;

public class AddMemberCommandHandler(
    PostgresDbContext postgres
    ) : ICommandHandler<AddMemberCommand, Member>
{
    public async Task<Member> HandleAsync(AddMemberCommand command)
    {
        if (command.RoomId is null or 0)
            throw new ArgumentException(Errors.Member.RoomIdIsInvalid);
        
        if (command.UserId is null or 0)
            throw new ArgumentException(Errors.Member.UserIdIsInvalid);

        command.Role ??= Role.Spectator;
        var m = new Member
        {
            UserId = command.UserId.Value,
            RoomId = command.RoomId.Value,
            Role = command.Role.Value
        };

        var result = await postgres.Members.AddAsync(m);
        return result.Entity;
    }
}