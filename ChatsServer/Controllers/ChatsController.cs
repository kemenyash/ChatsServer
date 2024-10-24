using ChatsServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatsServer.Controllers
{
    [Route("api/{token}/chats")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly DataScope dataPresenter;
        private readonly ProtectionService protectionService;

        public ChatsController(DataScope dataPresenter, ProtectionService protectionService)
        {
            this.dataPresenter = dataPresenter;
            this.protectionService = protectionService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string token)
        {
            if(!protectionService.CheckToken(token)) return Unauthorized();

            var chats = await dataPresenter.GetChats();
            return Ok(chats);
        }
    }
}
