using Backend.Domain.Abstractions.Commands;

namespace Backend.Application.Commands.Games;

public class CreateGameCommand : ICommand<long>
{
    public long UserId { get; set; }
    public long MaxRating { get; set; }
}