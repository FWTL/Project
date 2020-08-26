using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Responses
{
    public class User
    {
        [JsonPropertyName("first_name")]
        public string Firstname { get; set; }

        [JsonPropertyName("last_name")]
        public string Lastname { get; set; }

        public string Username { get; set; }
    }
}