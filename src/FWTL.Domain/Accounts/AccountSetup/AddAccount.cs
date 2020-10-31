using System;
using System.Threading.Tasks;
using Automatonymous;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using FWTL.EventHandlers;
using GreenPipes;

namespace FWTL.Domain.Accounts
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

            public Guid UserId { get; set; }
        }

        public class Handler : Activity<AccountSetupState, Command>
        {
            private readonly IGuidService _guidService;
            private readonly IAggregateStore _aggregateStore;

            public Handler(IGuidService guidService, IAggregateStore aggregateStore)
            {
                _guidService = guidService;
                _aggregateStore = aggregateStore;
            }

            public void Accept(StateMachineVisitor visitor)
            {
                throw new NotImplementedException();
            }

            public async Task Execute(BehaviorContext<AccountSetupState, Command> context, Behavior<AccountSetupState, Command> next)
            {
                var account = _aggregateStore.GetNew<AccountAggregate>();
                account.Create(_guidService.New, context.Data);
                await next.Execute(context);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<AccountSetupState, Command, TException> context, Behavior<AccountSetupState, Command> next) where TException : Exception
            {
                throw new NotImplementedException();
            }

            public void Probe(ProbeContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}