using Backend.Domain.Entities;
using Backend.Domain.Enums;

namespace Backend.Data.QueryHandlers.Games;

public class GetAllGamesDto
{
    public long Id { get; set; }
    public long MaxRating { get; set; }
    public DateTime CreationDate { get; set; }
    public RoomStatus Status { get; set; }
}