using System;
using System.Threading.Tasks;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class GenerateInfrastructure
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
            private readonly IAggregateStore _aggregateStore;
            private readonly IInfrastructureService _infrastructureSetupService;

            public Handler(IAggregateStore aggregateStore, IInfrastructureService infrastructureService)
            {
                _aggregateStore = aggregateStore;
                _infrastructureSetupService = infrastructureService;
            }

            public async Task<IAggregateRoot> ExecuteAsync(Command command)
            {
                AccountAggregate account = await _aggregateStore.GetByIdAsync<AccountAggregate>(command.AccountId);
                account.TryToCreateInfrastructure();

                var result = await _infrastructureSetupService.GenerateTelegramApi(command.AccountId);
                if (result.IsSuccess)
                {
                    account.GenerateInfrastructure();
                    return account;
                }

                account.FailSetup(result.Errors);
                return account;
            }
        }
    }
}