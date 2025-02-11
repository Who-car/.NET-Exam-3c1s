using Backend.Domain.Abstractions.Queries;
using Backend.Domain.Entities;

namespace Backend.Data.QueryHandlers.Rooms;

public class GetRoomQueryHandler(
    PostgresDbContext postgres
    ) : IQueryHandler<GetRoom, Room?>
{
    public async Task<Room?> HandleAsync(GetRoom query) =>
        await postgres.Rooms.FindAsync(query.Id);
}