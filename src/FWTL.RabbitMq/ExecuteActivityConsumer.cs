using System;
using System.Threading.Tasks;
using FWTL.Core.Events;
using FWTL.Core.Helpers;
using MassTransit;
using MassTransit.Courier;

namespace FWTL.RabbitMq
{
    public class ExecuteActivitySaga<TState,TEvent> : IExecuteActivity<TEvent> where TEvent : class, IEvent
    {
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IEventHandler<TEvent> _handler;

        public ExecuteActivitySaga(
            IEventHandler<TEvent> handler,
            IExceptionHandler exceptionHandler)
        {
            _handler = handler;
            _exceptionHandler = exceptionHandler;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<TEvent> context)
        {
            try
            {
                await _handler.HandleAsync(context.Arguments);
                return context.Completed();
            }
            catch (Exception ex)
            {
                _exceptionHandler.Handle(ex, context.Arguments);
                return context.Faulted();
            }
        }
    }
}
