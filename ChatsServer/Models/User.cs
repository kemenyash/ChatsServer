using Newtonsoft.Json;

namespace ChatsServer.Models
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("telegram_chat_id")]
        public long ChatId { get; set; }
        [JsonProperty("img_url")]
        public string ImgUrl { get; set; }
    }
}
