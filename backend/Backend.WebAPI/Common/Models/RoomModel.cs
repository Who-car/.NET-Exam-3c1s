using Backend.Domain.Entities;

namespace Backend.WebAPI.Common.Models;

public class RoomModel
{
    public long Id { get; set; }
    public DateTime LastRoundDateTime { get; set; }

    public List<MemberModel> Members { get; set; } = [];
    public List<MoveModel> Moves { get; set; } = [];
}