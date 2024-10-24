using ChatsServer.Models;
using ChatsServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ChatsServer.Controllers
{
    [Route("api/{token}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly DataScope dataPresenter;
        private readonly TelegramBotService telegramBotService;
        private readonly ProtectionService protectionService;

        public MessagesController(DataScope dataPresenter, TelegramBotService telegramBotService, ProtectionService protectionService)
        {
            this.dataPresenter = dataPresenter;
            this.telegramBotService = telegramBotService;
            this.protectionService = protectionService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string token, int userId)
        {
            if (!protectionService.CheckToken(token)) return Unauthorized();
            var chats = await dataPresenter.GetMessages(userId);
            return Ok(chats);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> SendMessage(string token, Message message)
        {
            if (!protectionService.CheckToken(token)) return Unauthorized();

            await dataPresenter.AddMessage(message);
            await telegramBotService.SendMessage(message);
            return Ok();
        }
    }
}
