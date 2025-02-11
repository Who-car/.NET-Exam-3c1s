using Backend.Domain.Abstractions.Queries;
using Backend.Domain.Entities;

namespace Backend.Data.QueryHandlers.Rooms;

public class GetRoom : IQuery<Room>
{
    public long Id { get; set; }
}