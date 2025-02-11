using Backend.Application.Common.Strings;
using Backend.Data;
using Backend.Domain.Abstractions.Commands;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Application.Commands.Games;

public class CreateGameCommandHandler(
    PostgresDbContext postgres
) : ICommandHandler<CreateGameCommand, long>
{
    public async Task<long> HandleAsync(CreateGameCommand command)
    {
        var user = await postgres.Users.FirstOrDefaultAsync(u => u.Id == command.UserId);
        if (user is null)
            throw new ArgumentException(Errors.Authentication.UserNotFound);
        
        var room = new Room
        {
            MaxRating = command.MaxRating,
            Status = RoomStatus.Waiting,
            CreationDate = DateTime.UtcNow,
        };
        var roomResult = await postgres.Rooms.AddAsync(room);
        
        var author = new Member
        {
            Role = Role.Creator,
            Room = roomResult.Entity,
            User = user
        };
        var authorResult = await postgres.Members.AddAsync(author);
        
        await postgres.SaveChangesAsync();
        return roomResult.Entity.Id;
    }
}