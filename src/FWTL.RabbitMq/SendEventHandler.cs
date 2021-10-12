using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Core.Events;

namespace FWTL.RabbitMq
{
    public abstract class SendEventHandler<TEvent, TCommand> : IEventHandler<TEvent> where TEvent : IEvent where TCommand : class, ICommand
    {
        private readonly ICommandDispatcher _commandDispatcher;

        protected SendEventHandler(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public abstract TCommand Map(TEvent @event);

        public async Task HandleAsync(TEvent @event)
        {
            var command = Map(@event);
            await _commandDispatcher.DispatchAsync<TCommand>(command);
        }
    }
}