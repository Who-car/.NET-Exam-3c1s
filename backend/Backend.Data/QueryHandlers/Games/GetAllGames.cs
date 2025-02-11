using Backend.Domain.Abstractions.Queries;
using Backend.Domain.Entities;

namespace Backend.Data.QueryHandlers.Games;

public class GetAllGames : IQuery<List<GetAllGamesDto>>
{
    public int Offset { get; set; }
    public int Limit { get; set; }
}