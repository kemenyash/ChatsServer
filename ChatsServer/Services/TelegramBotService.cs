using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatsServer.Services
{
    public class TelegramBotService
    {
        private TelegramBotClient bot;
        private readonly string token;
        private readonly IServiceProvider serviceProvider;
        private CancellationTokenSource cts;

        public TelegramBotService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            token = Environment.GetEnvironmentVariable("TelegramToken");
            Initialization().ConfigureAwait(false);
        }

        private async Task Initialization()
        {
            cts = new CancellationTokenSource();
            bot = new TelegramBotClient(token);

            if(!await bot.TestApiAsync())
            {
                Console.WriteLine("Bot was crashed");
                return;
            }

            bot.StartReceiving(receiverOptions: new Telegram.Bot.Polling.ReceiverOptions
            {
                AllowedUpdates = new Telegram.Bot.Types.Enums.UpdateType[]
                {
                    Telegram.Bot.Types.Enums.UpdateType.Message
                }
            },
                cancellationToken: cts.Token,
                pollingErrorHandler: HandlePollingError,
                updateHandler: HandleUpdate);

            Console.WriteLine($"Bot {(await bot.GetMeAsync()).Username} was started");
        }

        private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(update.Message != null && update.Message.Type == MessageType.Text)
            {
                await AddNewMessage(update);
            }
        }

        private async Task AddNewMessage(Update update)
        {
            var chatId = update.Message.Chat.Id;
            using (var scope = serviceProvider.CreateScope())
            {
                var dataPresenter = scope.ServiceProvider.GetRequiredService<DataScope>();
                var user = await dataPresenter.GetUser(chatId);

                int userId = 0;
                if(user is null)
                {
                    userId = await RegisterUser(new Models.User
                    {
                        ChatId = chatId,
                        ImgUrl = await GetTelegramPhotoUrl(chatId),
                        Name = update.Message.Chat.Username
                    });
                }
                else
                {
                    userId = user.Id;
                }

                await dataPresenter.AddMessage(new Models.Message
                {
                    AddedTime = DateTime.UtcNow,
                    IsOperatorMessage = false,
                    UserId = userId,
                    Value = update.Message.Text
                });
            }
        }


        private async Task<int> RegisterUser(Models.User user)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dataPresenter = scope.ServiceProvider.GetRequiredService<DataScope>();
                var userId = await dataPresenter.AddUser(user);
                return userId;
            }

        }

        private async Task<string> GetTelegramPhotoUrl(long userId)
        {
            var userProfilePhotos = await bot.GetUserProfilePhotosAsync(userId);

            if (userProfilePhotos.TotalCount > 0)
            {
                var photo = userProfilePhotos.Photos[0];
                var fileId = photo.Last().FileId;

                var file = await bot.GetFileAsync(fileId);

                string fileUrl = $"https://api.telegram.org/file/bot{token}/{file.FilePath}";

                return fileUrl;
            }
            else
            {
                return string.Empty;
            }
        }

        private Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine("Error occured: " + exception.Message);
            return Task.CompletedTask;
        }

        public async Task SendMessage(Models.Message message)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dataPresenter = scope.ServiceProvider.GetRequiredService<DataScope>();
                var user = await dataPresenter.GetUser(message.UserId);
                try
                {
                    await bot.SendTextMessageAsync(chatId: user.ChatId, text: message.Value);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Error while sending message: {ex.Message}");
                }

            }
        }

        public void Invoke() 
        {
            Console.WriteLine("Trying to start bot...");
        }
    }
}
