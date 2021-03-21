using System.Text.Json.Serialization;

namespace FWTL.Core.Services.Dto
{
    public class InputChannel
    {
        [JsonPropertyName("channel_id")]
        public int ChannelId { get; set; }
    }
}