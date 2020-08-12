using Newtonsoft.Json;

namespace FWTL.TelegramClient.Responses
{
    public class UserProfilePhoto : IUserProfilePhoto
    {
        [JsonProperty("photo_id")]
        public long photo_id { get; set; }

        [JsonProperty("dc_id")]
        public int dc_id { get; set; }
    }
}