using System;
using System.Threading.Tasks;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class AddAccount
    {
        public class Request : IRequest
        {
            public string ExternalAccountId { get; set; }
        }

        public class Command : Request, ICommand
        {
            public Command()
            {
            }

            public Command(ICurrentUserService currentUserService)
            {
                UserId = currentUserService.CurrentUserId;
            }

            public Guid CorrelationId { get; set; }

            public Guid UserId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly IGuidService _guidService;
            private readonly IAggregateStore _aggregateStore;

            public Handler(IGuidService guidService, IAggregateStore aggregateStore)
            {
                _guidService = guidService;
                _aggregateStore = aggregateStore;
            }

            public Task<IAggregateRoot> ExecuteAsync(Command command)
            {
                var account = _aggregateStore.GetNew<AccountAggregate>();
                account.Create(_guidService.New, command);
                return Task.FromResult<IAggregateRoot>(account);
            }
        }
    }
}