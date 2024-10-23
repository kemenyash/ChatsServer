
using ChatsServer.Hubs;
using ChatsServer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatsServer.Services
{
    public class DataScope
    {
        private readonly DataStore.DataContext dataContext;
        private readonly IHubContext<ChatsHub> hubContext;

        public DataScope(DataStore.DataContext dataContext, IHubContext<ChatsHub> hubContext)
        {
            this.dataContext = dataContext;
            this.hubContext = hubContext;
        }

        public async Task<User> GetUser(long chatId)
        {
            var user = await dataContext.Users.FirstOrDefaultAsync(x => x.ChatId == chatId);
            if(user != null)
            {
                return new User
                {
                    ChatId = chatId,
                    Id = user.Id,
                    ImgUrl = user.Avatar,
                    Name = user.UserName
                };
            }

            return null;
        }

        public async Task<IEnumerable<Message>> GetMessages(int userId)
        {
            return await dataContext.Messages.Where(m => m.UserId == userId)
                                       .Select(m => new Message
                                       {
                                           AddedTime = m.AddedTime,
                                           Id = m.Id,
                                           IsOperatorMessage = m.User.IsOperator,
                                           UserId = userId,
                                           Value = m.Value
                                       }).ToListAsync() ?? Enumerable.Empty<Message>();
        }

        public async Task<IEnumerable<User>> GetChats() 
        {
            return await dataContext.Users.Where(x => !x.IsOperator)
                                    .Select(x => new User
                                    {
                                        ChatId = x.ChatId,
                                        Id = x.Id,
                                        ImgUrl = x.Avatar,
                                        Name = x.UserName
                                    }).ToListAsync()  ?? Enumerable.Empty<User>();
        }


        public async Task<int> AddUser(User user)
        {
            var userData = new DataStore.User
            {
                Avatar = user.ImgUrl,
                ChatId = user.ChatId,
                IsOperator = false,
                UserName = user.Name
            };

            await dataContext.Users.AddAsync(userData);
            await dataContext.SaveChangesAsync();
            user.Id = userData.Id;
            await hubContext.Clients.All.SendAsync("NewUser", user);
            return userData.Id;
        }

        public async Task<int> AddMessage(Message message)
        {
            var messageData = new DataStore.Message
            {
                AddedTime = message.AddedTime,
                UserId = message.UserId,
                Value = message.Value
            };

            await dataContext.Messages.AddAsync(messageData);
            await dataContext.SaveChangesAsync();
            message.Id = messageData.Id;
            await hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            return messageData.Id;
        }

        public async Task SendMessageFromOperator(Message message)
        {
            var messageId = await AddMessage(message);

        }
    }
}
