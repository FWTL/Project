using System.Threading.Tasks;
using FWTL.Core.Events;
using FWTL.Core.Services.Telegram;
using FWTL.Domain.Events;

namespace FWTL.RabbitMq
{
    public class AccountDeletedHandler : IEventHandler<AccountDeleted>
    {
        private readonly ITelegramClient _telegramClient;

        public AccountDeletedHandler(ITelegramClient telegramClient)
        {
            _telegramClient = telegramClient;
        }

        public async Task HandleAsync(AccountDeleted @event)
        {
            await _telegramClient.SystemService.RemoveSessionAsync(@event.AccountId);
        }
    }
}