using System;
using System.Threading.Tasks;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.TelegramClient;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class SendCode
    {
        public class Command : ICommand
        {
            public Command()
            {
            }

            public Guid AccountId { get; set; }
            public Guid CorrelationId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly ITelegramClient _telegramClient;
            private readonly IAggregateStore _aggregateStore;

            public Handler(ITelegramClient telegramClient, IAggregateStore aggregateStore)
            {
                _telegramClient = telegramClient;
                _aggregateStore = aggregateStore;
            }

            public async Task<IAggregateRoot> ExecuteAsync(Command command)
            {
                AccountAggregate account = await _aggregateStore.GetByIdAsync<AccountAggregate>(command.AccountId);
                account.SendCode();
                await _telegramClient.UserService.PhoneLoginAsync(account.SessionName, account.ExternalAccountId);

                return account;
            }
        }
    }
}