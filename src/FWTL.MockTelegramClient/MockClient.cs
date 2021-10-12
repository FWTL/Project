using FWTL.Core.Services.Telegram;
using FWTL.MockTelegramClient.Services;

namespace FWTL.MockTelegramClient
{
    public class MockClient : ITelegramClient
    {
        public IUserService UserService { get; }

        public ISystemService SystemService { get; }

        public IContactService ContactService { get; }

        public IMessageService MessageService { get; }

        public MockClient()
        {
            UserService = new UserService();
            SystemService = new SystemService();
            ContactService = new ContactService();
            MessageService = new MessageService();
        }
    }
}