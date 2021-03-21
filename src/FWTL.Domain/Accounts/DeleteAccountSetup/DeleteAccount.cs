using System;
using System.Threading.Tasks;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services;

namespace FWTL.Domain.Accounts.DeleteAccountSetup
{
    public class DeleteAccount
    {
        public class Request : IRequest
        {
            public Guid AccountId { get; set; }
        }

        public class Command : ICommand
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
            private readonly IAggregateStore _aggregateStore;
            private readonly IGuidService _guidService;

            public Handler(IAggregateStore aggregateStore, IGuidService guidService)
            {
                _aggregateStore = aggregateStore;
                _guidService = guidService;
            }

            public Task<IAggregateRoot> ExecuteAsync(Command command)
            {
                throw new NotImplementedException();
            }
        }
    }
}