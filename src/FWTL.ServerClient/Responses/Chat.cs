using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Responses
{
    public class Chat
    {
        public int Id { get; set; }

        [JsonPropertyName("_")]
        public string Type { get; set; }

        public string Title { get; set; }

        [JsonPropertyName("migrated_to")]
        public InputChannel MigratedTo { get; set; }
    }

    public class InputChannel
    {
        [JsonPropertyName("channel_id")]
        public int ChannelId { get; set; }
    }
}