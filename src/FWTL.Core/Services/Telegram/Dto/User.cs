using System.Text.Json.Serialization;

namespace FWTL.Core.Services.Telegram.Dto
{
    public class User
    {
        public int Id { get; set; }

        [JsonPropertyName("first_name")]
        public string Firstname { get; set; }

        [JsonPropertyName("last_name")]
        public string Lastname { get; set; }

        public string Username { get; set; }

        [JsonPropertyName("deleted")]
        public bool IsDeleted { get; set; }
    }
}