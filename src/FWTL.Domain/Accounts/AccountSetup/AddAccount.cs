using System;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using FWTL.Core.Validation;

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
            private readonly IAggregateStore _aggregateStore;

            public Handler(IAggregateStore aggregateStore)
            {
                _aggregateStore = aggregateStore;
            }

            public async Task<IAggregateRoot> ExecuteAsync(Command command)
            {
                var account = _aggregateStore.GetNew<AccountAggregate>();
                account.Create(command);
                return account;
            }
        }

        public class Validator : AppAbstractValidation<Command>
        {
            public Validator()
            {
                RuleFor(x => x.ExternalAccountId).NotEmpty().Matches("^[0-9]");
            }
        }
    }
}