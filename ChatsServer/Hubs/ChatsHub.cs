using ChatsServer.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatsServer.Hubs
{
    public class ChatsHub : Hub
    {
        public async Task ReceiveMessage(Message message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task NewUser(User user)
        {
            await Clients.All.SendAsync("NewUser", user);
        }
    }
}
