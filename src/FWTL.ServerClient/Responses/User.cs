using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Responses
{
    public class User
    {
        public int Id { get; set; }

        [JsonPropertyName("first_name")]
        public string Firstname { get; set; }

        [JsonPropertyName("last_name")]
        public string Lastname { get; set; }

        public string Username { get; set; }
    }
}