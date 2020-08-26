using FWTL.TelegramClient.Responses;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FWTL.TelegramClient.Converters
{
    public class GetSelfConverter : JsonConverter<User>
    {
        public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            User user = new User();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return user;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "first_name":
                            user.Firstname = reader.GetString();
                            break;

                        case "last_name":
                            user.Lastname = reader.GetString();
                            break;

                        case "Username":
                            user.Username = reader.GetString();
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}