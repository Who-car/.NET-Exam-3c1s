using Backend.Domain.Abstractions.Queries;
using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.QueryHandlers.Games;

public class GetAllGamesQueryHandler(
    PostgresDbContext postgres
    ) : IQueryHandler<GetAllGames, List<GetAllGamesDto>>
{
    public async Task<List<GetAllGamesDto>> HandleAsync(GetAllGames q)
    {
        // Получение данных
        var books = postgres.Rooms;;

        // Сортировка
        var ordered = books
            .OrderByDescending(b => b.CreationDate)
            .ThenBy(b => b.Status);
        
        // Пагинация
        var offset = (q.Offset - 1) * q.Limit < 0 ? 0 : (q.Offset - 1) * q.Limit;
        var paginated = ordered
            .Skip(offset).Take(q.Limit);

        var queried = await paginated.ToListAsync();

        return queried.Select(room => new GetAllGamesDto()
            {
                Id = room.Id, CreationDate = room.CreationDate, Status = room.Status, MaxRating = room.MaxRating,
            })
            .ToList();
    }
}