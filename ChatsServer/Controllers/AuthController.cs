﻿using ChatsServer.Models;
using ChatsServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ProtectionService protectionService;

        public AuthController(ProtectionService protectionService)
        {
            this.protectionService = protectionService;
        }

        [HttpPost]
        public async Task<IActionResult> Auth([FromBody] AuthCredentials credentials)
        {
            if (!await protectionService.CheckAuthCredentials(credentials.Username, credentials.Password)) 
            {
                return Unauthorized();
            }
            var updatedToken = await protectionService.UpdateToken(credentials.Username);
            return Ok(updatedToken);
        }
    }
}