using FWTL.TelegramClient.Converters;
using FWTL.TelegramClient.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace FWTL.TelegramClient
{
    public class Client : ITelegramClient
    {
        public IUserService UserService { get; }

        public ISystemService SystemService { get; }

        public Client(string url)
        {
            IRestClient client = new RestClient(url);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new TypeConverter());

            client.UseNewtonsoftJson(settings);

            UserService = new UserService(client);
            SystemService = new SystemService(client);
        }
    }
}