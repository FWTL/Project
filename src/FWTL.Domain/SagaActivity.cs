using System;
using System.Threading.Tasks;
using Automatonymous;
using FWTL.Core.Commands;
using GreenPipes;

namespace FWTL.Domain
{
    public class SagaActivity<TState, TCommand> : Activity<TState, TCommand> where TCommand : class, ICommand where TState : ISagaState
    {
        private readonly ISagaConsumer<TCommand> _sagaConsumer;

        public SagaActivity(ISagaConsumer<TCommand> sagaConsumer)
        {
            _sagaConsumer = sagaConsumer;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<TState, TCommand> context, Behavior<TState, TCommand> next)
        {
            await _sagaConsumer.Consume(context);
            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TState, TCommand, TException> context, Behavior<TState, TCommand> next) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }
    }
}