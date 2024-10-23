using ChatsServer.Hubs;
using ChatsServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IHubContext<ChatsHub> hubContext;

        public TestController(IHubContext<ChatsHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> TestContacts()
        {
            await hubContext.Clients.All.SendAsync("NewUser", new User
            {
                ChatId = 1,
                Id = 1,
                Name = "Test"
            });

            return Ok();
        }
    }
}
