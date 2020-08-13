using FWTL.TelegramClient.Types;
using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Responses
{
    public class UserProfilePhoto : IUserProfilePhoto
    {
        [JsonPropertyName("photo_id")]
        public long photo_id { get; set; }

        [JsonPropertyName("dc_id")]
        public int dc_id { get; set; }
    }
}