﻿using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Commands;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using MassTransit;

namespace FWTL.RabbitMq
{
    public class CommandConsumer<TCommand> : IConsumer<TCommand> where TCommand : class, ICommand
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICommandHandlerAsync<TCommand> _handler;

        public CommandConsumer()
        {
                
        }

        //public CommandConsumer(
        //    ICommandHandlerAsync<TCommand> handler,
        //    IEventDispatcher eventDispatcher)
        //{
        //    _handler = handler;
        //    _eventDispatcher = eventDispatcher;
        //}

        public async Task Consume(ConsumeContext<TCommand> context)
        {
            try
            {
                await _handler.ExecuteAsync(context.Message);
                await context.RespondAsync(new Response());
            }
            catch (ValidationException ex)
            {
                await context.RespondAsync(new Response(ex));
            }
        }
    }
}