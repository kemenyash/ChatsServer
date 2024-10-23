
using ChatsServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatsServer.Services
{
    public class DataPresenter
    {
        private readonly DataStore.DataContext dataContext;

        public DataPresenter(DataStore.DataContext dataContext)
        {
            this.dataContext = dataContext;
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
    }
}
