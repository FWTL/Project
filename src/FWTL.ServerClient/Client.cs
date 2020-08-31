using FWTL.TelegramClient.Services;
using System.Net.Http;

namespace FWTL.TelegramClient
{
    public class Client : ITelegramClient
    {
        public IUserService UserService { get; }

        public ISystemService SystemService { get; }

        public IContactService ContactService { get; }

        public IMessageService MessageService { get; }

        public Client(HttpClient client)
        {
            UserService = new UserService(client);
            SystemService = new SystemService(client);
            ContactService = new ContactService(client);
            MessageService = new MessageService(client);
        }
    }
}