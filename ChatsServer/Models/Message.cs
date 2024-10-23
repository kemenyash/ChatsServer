using Newtonsoft.Json;

namespace ChatsServer.Models
{
    public class Message
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("user_id")]
        public int UserId { get; set; }
        [JsonProperty("is_operator_message")]
        public bool IsOperatorMessage { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("added_time")]
        public DateTime AddedTime { get; set; }
    }
}
