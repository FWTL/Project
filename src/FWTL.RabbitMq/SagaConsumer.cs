﻿using System;
using System.Threading.Tasks;
using Automatonymous;
using FluentValidation;
using FWTL.Common.Commands;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Helpers;
using FWTL.TelegramClient.Exceptions;
using MassTransit;

namespace FWTL.RabbitMq
{
    public class SagaConsumer<TCommand> : ISagaConsumer<TCommand> where TCommand : class, ICommand
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ICommandHandler<TCommand> _handler;
        private readonly IEventFactory _eventFactory;
        private readonly IAggregateStore _aggregateStore;

        public SagaConsumer(
            ICommandHandler<TCommand> handler,
            IEventFactory eventFactory,
            IEventDispatcher eventDispatcher,
            IExceptionHandler exceptionHandler,
            IAggregateStore aggregateStore)
        {
            _handler = handler;
            _eventFactory = eventFactory;
            _eventDispatcher = eventDispatcher;
            _exceptionHandler = exceptionHandler;
            _aggregateStore = aggregateStore;
        }

        public async Task Consume<TState>(BehaviorContext<TState, TCommand> context)
        {
            try
            {
                IAggregateRoot aggregateRoot = await _handler.ExecuteAsync(context.Data);
                _eventFactory.Make(aggregateRoot.Events);

                await aggregateRoot.CommitAsync(_aggregateStore);

                foreach (var composite in aggregateRoot.Events)
                {
                    await context.Publish(composite.Event, composite.Event.GetType());
                }
            }
            catch (ValidationException ex)
            {
                await context.RespondAsync(new Response(ex));
            }
            catch (TelegramClientException ex)
            {
                await context.RespondAsync(new Response(ex));
            }
            catch (Exception ex)
            {
                var exceptionId = _exceptionHandler.Handle(ex, context.Data);
                await context.RespondAsync(message: new Response(exceptionId, ex));
            }
        }
    }
}