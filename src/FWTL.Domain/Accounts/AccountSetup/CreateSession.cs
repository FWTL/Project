using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;
using FWTL.Core.Commands;
using FWTL.EventHandlers;
using GreenPipes;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class CreateSession
    {
        public class Command : ICommand
        {
            public Command()
            {
            }

            public Guid UserId { get; set; }
        }

        public class Handler : Activity<AccountSetupState, Command>
        {
            public void Accept(StateMachineVisitor visitor)
            {
                throw new NotImplementedException();
            }

            public Task Execute(BehaviorContext<AccountSetupState, Command> context, Behavior<AccountSetupState, Command> next)
            {
                throw new NotImplementedException();
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
