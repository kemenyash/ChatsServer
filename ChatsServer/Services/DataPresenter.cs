
using ChatsServer.Models;

namespace ChatsServer.Services
{
    public class DataPresenter
    {
        private readonly DataStore.DataContext dataContext;

        public DataPresenter(DataStore.DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public IEnumerable<Message> GetMessages(int userId)
        {
            return dataContext.Messages.Where(m => m.UserId == userId)
                                       .Select(m => new Message
                                       {
                                           AddedTime = m.AddedTime,
                                           Id = m.Id,
                                           IsOperatorMessage = m.User.IsOperator,
                                           UserId = userId,
                                           Value = m.Value
                                       }) ?? Enumerable.Empty<Message>();
        }

        public IEnumerable<User> GetChats() 
        {
            return dataContext.Users.Where(x => !x.IsOperator)
                                    .Select(x => new User
                                    {
                                        ChatId = x.ChatId,
                                        Id = x.Id,
                                        ImgUrl = x.Avatar,
                                        Name = x.UserName
                                    }) ?? Enumerable.Empty<User>();
        }
    }
}
