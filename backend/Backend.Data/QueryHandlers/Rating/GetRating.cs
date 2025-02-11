using Backend.Domain.Abstractions.Queries;

namespace Backend.Data.QueryHandlers.Rating;

public class GetRating : IQuery<long>
{
    public long UserId { get; set; }
}