using System.Text.Json.Serialization;
using FWTL.Core.Services.Types;

namespace FWTL.Core.Services.Telegram.Dto
{
    public class UserProfilePhoto : IUserProfilePhoto
    {
        [JsonPropertyName("photo_id")]
        public long photo_id { get; set; }

        [JsonPropertyName("dc_id")]
        public int dc_id { get; set; }
    }
}