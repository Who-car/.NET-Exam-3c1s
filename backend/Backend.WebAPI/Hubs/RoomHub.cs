using Backend.WebAPI.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;

namespace Backend.WebAPI.Hubs;

public class RoomHub : Hub<IRoomClient>
{
    
}