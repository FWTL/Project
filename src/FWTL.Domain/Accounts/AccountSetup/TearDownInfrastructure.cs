using System;
using System.Threading.Tasks;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class TearDownInfrastructure
    {
        public class Command : ICommand
        {
            public Guid AccountId { get; set; }
            public Guid CorrelationId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly IInfrastructureService _infrastructureService;

            public Handler(IInfrastructureService infrastructureService)
            {
                _infrastructureService = infrastructureService;
            }

            private readonly IAggregateStore _aggregateStore;

            public Handler(IAggregateStore aggregateStore, IInfrastructureService infrastructureService)
            {
                _aggregateStore = aggregateStore;
                _infrastructureService = infrastructureService;
            }

            public async Task<IAggregateRoot> ExecuteAsync(Command command)
            {
                var account = await _aggregateStore.GetByIdAsync<AccountAggregate, Command>(command.AccountId, command);

                var result = await _infrastructureService.TearDownTelegramApi(command.AccountId);
                if (result.IsSuccess)
                {
                    account.TearDownInfrastructure();
                    return account;
                }

                account.FailSetup(result.Errors);
                return account;
            }
        }
    }
}