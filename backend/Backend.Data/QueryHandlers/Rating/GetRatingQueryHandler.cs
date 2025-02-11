using Backend.Data.QueryHandlers.Rooms;
using Backend.Domain.Abstractions.Queries;
using Backend.Domain.Entities;

namespace Backend.Data.QueryHandlers.Rating;

public class GetRatingQueryHandler(
    MongoDbContext mongo
) : IQueryHandler<GetRating, long>
{
    public async Task<long> HandleAsync(GetRating query)
    {
        // var collection = mongo.GetCollection<Rating>("Ratings");
        // collection.FindAsync(rating => rating.UserId == query.UserId);

        return 2000;
    }
}