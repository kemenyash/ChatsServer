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

        public MessagesController(DataScope dataPresenter)
        {
            this.dataPresenter = dataPresenter;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string token, int userId)
        {
            var chats = await dataPresenter.GetMessages(userId);
            return Ok(chats);
        }

        [HttpPost("{chatId}")]
        public async Task<IActionResult> SendMessage(Message message)
        {
            await dataPresenter.AddMessage(message);
            return Ok();
        }
    }
}
