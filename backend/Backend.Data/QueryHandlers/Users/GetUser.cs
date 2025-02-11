using Backend.Domain.Abstractions.Queries;
using Backend.Domain.Entities;

namespace Backend.Data.QueryHandlers.Users;

public class GetUser : IQuery<User>
{
    public long? Id { get; set; }
    public string? Username { get; set; }
}