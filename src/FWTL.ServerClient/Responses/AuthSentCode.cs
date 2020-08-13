using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Responses
{
    public class AuthSentCode
    {
        [JsonPropertyName("phone_code_hash")]
        public string PhoneCodeHash { get; set; }

        public int? Timeout { get; set; }
    }
}