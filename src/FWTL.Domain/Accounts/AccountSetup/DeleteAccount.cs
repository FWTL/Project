using System;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Helpers;
using FWTL.Common.Validators;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class DeleteAccount
    {
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
                var account = _aggregateStore.GetNew<AccountAggregate>();
                account.Create(_guidService.New, command);
                return Task.FromResult<IAggregateRoot>(account);
            }
        }

        public class Validator : AppAbstractValidation<Command>
        {
            public Validator()
            {
                RuleFor(x => x.ExternalAccountId).NotEmpty().Matches(RegexExpressions.OnlyNumbers).MaximumLength(20);
            }
        }
    }
}