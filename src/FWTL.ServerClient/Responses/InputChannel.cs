using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Responses
{
    public class InputChannel
    {
        [JsonPropertyName("channel_id")]
        public int ChannelId { get; set; }
    }
}