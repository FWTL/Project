using NodaTime;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Converters
{
    internal class DateConverter : JsonConverter<Instant>
    {
        public override Instant Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Instant.FromUnixTimeSeconds(reader.GetInt32());
        }

        public override void Write(Utf8JsonWriter writer, Instant value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}