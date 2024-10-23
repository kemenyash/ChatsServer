using ChatsServer.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatsServer.Hubs
{
    public class ChatsHub : Hub
    {
        // Подія для надсилання повідомлень
        public async Task SendMessage(Message message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        // Подія для додавання нового користувача
        public async Task AddUser(User user)
        {
            await Clients.All.SendAsync("NewUser", user);
        }
    }
}
