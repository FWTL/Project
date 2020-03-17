using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace FWTL.TelegramClient
{
    public class TelegramClient : ITelegramClient
    {
        public IUserService UserService { get; }

        public ISystemService SystemService { get; }

        public TelegramClient(string url)
        {
            IRestClient client = new RestClient(url);
            client.UseNewtonsoftJson();

            UserService = new UserService(client);
            SystemService = new SystemService(client);
        }
    }
}