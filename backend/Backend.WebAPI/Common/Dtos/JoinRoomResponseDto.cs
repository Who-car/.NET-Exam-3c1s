namespace Backend.WebAPI.Common.Dtos;

public class JoinRoomResponseDto
{
    public bool JoinGame { get; set; }
    public string WinnerName { get; set; }
    public long WinnerNewRating { get; set; }
    
    public string LoserName { get; set; }
    public long LoserNewRating { get; set; }
}