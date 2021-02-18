using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;

namespace FWTL.Core.Commands
{
    public interface ISagaConsumer<in TCommand> where TCommand : class, ICommand
    {
        public Task Consume<TState>(BehaviorContext<TState, TCommand> context) where TState : ISagaState;
    }
}
