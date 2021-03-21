using System;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services.Telegram;
using FWTL.Core.Services.Telegram.Dto;
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
                ResponseWrapper response = await _telegramClient.UserService.PhoneLoginAsync(account.Id.ToString(), account.ExternalAccountId);

                if (response.IsSuccess)
                {
                    account.SendCode();
                    return account;
                }

                account.FailSetup(response.Errors.Select(e => e.Message));
                return account;
            }
        }
    }
}