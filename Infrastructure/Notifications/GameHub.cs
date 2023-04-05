using Microsoft.AspNetCore.SignalR;


namespace Infrastructure.Notifications
{
    public class GameHub : Hub
    {
        public GameHub()
        {
        }
        public async Task NotifyOtherPlayers()
        {
            await Clients.AllExcept(Context.ConnectionId).SendAsync("GameOver");
        }

        public async Task NotifyOtherPlayersFromFrontEnd()
        {
            await Clients.Others.SendAsync("GameOver");
        }
    }
}