using FWTL.TelegramClient.Types;
using Newtonsoft.Json;

namespace FWTL.TelegramClient.Responses
{
    public class User
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        public string UserName { get; set; }

        public IUserProfilePhoto Photo { get; set; }
    }
}