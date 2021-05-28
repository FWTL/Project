using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services.Telegram;
using FWTL.Events;

namespace FWTL.Domain.Accounts.DeleteAccountSetup
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
            await _telegramClient.SystemService.RemoveSessionAsync(@event.AccountId.ToString());
        }
    }

    public abstract class SendEventHandler<TEvent, TCommand> : IEventHandler<TEvent> where TEvent : IEvent where TCommand : class, ICommand
    {
        private readonly ICommandDispatcher _commandDispatcher;

        protected SendEventHandler(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public abstract TCommand Map(TEvent @event);

        public async Task HandleAsync(TEvent @event)
        {
            var command = Map(@event);
            await _commandDispatcher.DispatchAsync<TCommand>(command);
        }
    }
}