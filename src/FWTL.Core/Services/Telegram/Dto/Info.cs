using System.Text.Json.Serialization;

namespace FWTL.Core.Services.Dto
{
    public class Info
    {
        [JsonPropertyName("User")]
        public User User { get; set; }

        [JsonPropertyName("Chat")]
        public Chat Chat { get; set; }
    }
}