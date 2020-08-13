using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Responses
{
    public class AuthAuthorization
    {
        [JsonPropertyName("tmp_sessions")]
        public int TmpSessions { get; set; }
    }
}