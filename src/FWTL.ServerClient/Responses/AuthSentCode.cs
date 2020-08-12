using Newtonsoft.Json;

namespace FWTL.TelegramClient.Responses
{
    public class AuthSentCode
    {
        [JsonProperty("phone_code_hash")]
        public string PhoneCodeHash { get; set; }

        public int? Timeout { get; set; }
    }
}