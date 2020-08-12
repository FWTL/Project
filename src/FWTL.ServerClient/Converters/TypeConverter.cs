using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using FWTL.TelegramClient.Responses;
using FWTL.TelegramClient.Services;

namespace FWTL.TelegramClient.Converters
{
    internal class TypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            var type = jsonObject["_"].ToString();
            var pascalCase = type.ToPascalCase();
            string typeFullName = $"FWTL.TelegramClient.Responses.{pascalCase}";
            var dotNetType = Type.GetType(typeFullName);

            return jsonObject.ToObject(dotNetType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsInterface;
        }

        public override bool CanWrite => false;
    }
}