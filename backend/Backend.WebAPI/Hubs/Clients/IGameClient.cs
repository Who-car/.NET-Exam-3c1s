namespace Backend.WebAPI.Hubs.Clients;

public interface IGameClient
{
    void OnUserConnected(string username);
    void OnUserDisconnected(string username);
    void OnMoveMade(string username);
    void OnWinnerCalculated(string winnerName, Dictionary<string, string> moves);
}