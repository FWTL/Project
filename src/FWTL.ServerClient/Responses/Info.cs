using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Responses
{
    public class Info
    {
      
        [JsonPropertyName("User")]
        public User User { get; set; }

        [JsonPropertyName("Chat")]
        public Chat Chat { get; set; }
    }
}