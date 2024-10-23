using ChatsServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatsServer.Controllers
{
    [Route("api/{token}/chats")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly DataPresenter dataPresenter;

        public ChatsController(DataPresenter dataPresenter) 
        {
            this.dataPresenter = dataPresenter;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string token)
        {
            var chats = await dataPresenter.GetChats();
            return Ok(chats);
        }
    }
}
