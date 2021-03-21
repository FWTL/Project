using System.Text.Json.Serialization;
using NodaTime;

namespace FWTL.Core.Services.Dto
{
    public class Chat
    {
        public int Id { get; set; }

        [JsonPropertyName("_")]
        public string Type { get; set; }

        public string Title { get; set; }

        [JsonPropertyName("migrated_to")]
        public InputChannel MigratedTo { get; set; }

        public Instant Date { get; set; }
    }
}