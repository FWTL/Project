using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;

namespace FWTL.TelegramClient.Converters
{
    public class DateConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Instant);
        }

        public override JsonConverter CreateConverter(Type typeToConvert,
            JsonSerializerOptions options)
        {
            return new DateConverter();
        }
    }
}