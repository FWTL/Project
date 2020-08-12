using Newtonsoft.Json;

namespace FWTL.TelegramClient.Responses
{
    public class AuthAuthorization
    {
        [JsonProperty("tmp_sessions")]
        public int TmpSessions { get; set; }
    }
}