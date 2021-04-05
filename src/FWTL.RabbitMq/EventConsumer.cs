using System;
using System.Threading.Tasks;
using FWTL.Core.Events;
using FWTL.Core.Helpers;
using MassTransit;

namespace FWTL.RabbitMq
{
    public class EventConsumer<TEvent> : IConsumer<TEvent> where TEvent : class, IEvent
    {
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IEventHandler<TEvent> _handler;

        public EventConsumer(
            IEventHandler<TEvent> handler,
            IExceptionHandler exceptionHandler)
        {
            _handler = handler;
            _exceptionHandler = exceptionHandler;
        }

        public async Task Consume(ConsumeContext<TEvent> context)
        {
            try
            {
                await _handler.HandleAsync(context.Message);
            }
            catch (Exception ex)
            {
                _exceptionHandler.Handle(ex, context.Message);
            }
        }
    }
}
