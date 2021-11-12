using System;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services.Telegram;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class CreateSession
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
                AccountAggregate account = await _aggregateStore.GetByIdAsync<AccountAggregate, Command>(command.AccountId, command);

                var response = await _telegramClient.SystemService.AddSessionAsync(account.Id);
                if (response.IsSuccess)
                {
                    account.CreateSession();
                    return account;
                }

                account.FailSetup(response.Errors.Select(e => e.Message));
                return account;
            }
        }
    }
}