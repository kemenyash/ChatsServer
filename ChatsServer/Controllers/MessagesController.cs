using ChatsServer.Models;
using ChatsServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatsServer.Controllers
{
    [Route("api/{token}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly DataScope dataPresenter;
        private readonly TelegramBotService telegramBotService;

        public MessagesController(DataScope dataPresenter, TelegramBotService telegramBotService)
        {
            this.dataPresenter = dataPresenter;
            this.telegramBotService = telegramBotService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string token, int userId)
        {
            var chats = await dataPresenter.GetMessages(userId);
            return Ok(chats);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> SendMessage(Message message)
        {
            await dataPresenter.AddMessage(message);
            await telegramBotService.SendMessage(message);
            return Ok();
        }
    }
}
