using Backend.Domain.Abstractions.Queries;
using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.QueryHandlers.Users;

public class GetUserQueryHandler(
    PostgresDbContext postgres
    ) : IQueryHandler<GetUser, User?>
{
    public async Task<User?> HandleAsync(GetUser query) =>
        await postgres.Users.FirstOrDefaultAsync(u => u.Id == query.Id || u.Username == query.Username);
}