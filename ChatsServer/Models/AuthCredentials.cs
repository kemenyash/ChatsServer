using Newtonsoft.Json;

namespace ChatsServer.Models
{
    public class AuthCredentials
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
