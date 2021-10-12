using System;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Cqrs.Responses;
using FWTL.Common.Exceptions;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Helpers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FWTL.RabbitMq
{
    public class CommandConsumer<TCommand> : IConsumer<TCommand> where TCommand : class, ICommand
    {
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ICommandHandler<TCommand> _handler;
        private readonly IEventFactory _eventFactory;
        private readonly IAggregateStore _aggregateStore;
        private readonly ILogger<CommandConsumer<TCommand>> _logger;

        public CommandConsumer(
            ICommandHandler<TCommand> handler,
            IEventFactory eventFactory,
            IExceptionHandler exceptionHandler,
            IAggregateStore aggregateStore,
            ILogger<CommandConsumer<TCommand>> logger)
        {
            _handler = handler;
            _eventFactory = eventFactory;
            _exceptionHandler = exceptionHandler;
            _aggregateStore = aggregateStore;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TCommand> context)
        {
            try
            {
                _logger.LogDebug($"{context.Message.GetType().Name} {context.Message.CorrelationId}");

                IAggregateRoot aggregateRoot = await _handler.ExecuteAsync(context.Message);
                _eventFactory.Make(aggregateRoot.Events);

                await aggregateRoot.CommitAsync(_aggregateStore);

                foreach (var composite in aggregateRoot.Events)
                {
                    await context.Publish(composite.Event, composite.Event.GetType());
                }

                await context.RespondAsync(new Common.Cqrs.Responses.Response(aggregateRoot.Id));
            }
            catch (ValidationException ex)
            {
                await context.RespondAsync(new BadRequestResponse(ex));
            }
            catch (TelegramClientException ex)
            {
                await context.RespondAsync(new BadRequestResponse(ex));
            }
            catch (Exception ex)
            {
                var exceptionId = _exceptionHandler.Handle(ex, context.Message);
                await context.RespondAsync(message: new ErrorResponse(exceptionId));
            }
        }
    }
}