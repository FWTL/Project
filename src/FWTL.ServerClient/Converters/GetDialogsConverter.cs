using FWTL.TelegramClient.Responses;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Converters
{
    public class GetDialogsConverter : JsonConverter<Dialog>
    {
        private static readonly Dictionary<string, Action<Dialog>> TypeMap = new Dictionary<string, Action<Dialog>>()
        {
            {"peerUser", dialog => dialog.Type = Dialog.DialogType.User},
            {"peerChat", dialog => dialog.Type = Dialog.DialogType.Chat},
            {"peerChannel", dialog => dialog.Type = Dialog.DialogType.Channel},
        };

        public override Dialog Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dialog = new Dialog();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dialog;
                }

                reader.Read(); //value
                TypeMap[reader.GetString()](dialog);

                reader.Read(); //property
                reader.Read(); //value
                dialog.Id = reader.GetInt32();
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Dialog value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}