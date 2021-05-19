using System;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Exceptions;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Helpers;
using MassTransit.Courier;

namespace FWTL.Domain.Accounts.Activities
{
    public class LogoutActivityArgs
    {
        public Guid AccountId { get; set; }
    }

    public class CommandActivity<TCommand> : IExecuteActivity<TCommand> where TCommand : class, ICommand
    {
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ICommandHandler<TCommand> _handler;
        private readonly IEventFactory _eventFactory;
        private readonly IAggregateStore _aggregateStore;

        public CommandActivity(
            ICommandHandler<TCommand> handler,
            IEventFactory eventFactory,
            IExceptionHandler exceptionHandler,
            IAggregateStore aggregateStore)
        {
            _handler = handler;
            _eventFactory = eventFactory;
            _exceptionHandler = exceptionHandler;
            _aggregateStore = aggregateStore;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<TCommand> context)
        {
            try
            {
                IAggregateRoot aggregateRoot = await _handler.ExecuteAsync(context.Arguments);
                _eventFactory.Make(aggregateRoot.Events);

                await aggregateRoot.CommitAsync(_aggregateStore);

                foreach (var composite in aggregateRoot.Events)
                {
                    await context.Publish(composite.Event, composite.Event.GetType());
                }

                return context.Completed();
            }
            catch (ValidationException ex)
            {
                return context.Faulted(ex);   
            }
            catch (TelegramClientException ex)
            {
                return context.Faulted(ex);
            }
            catch (Exception ex)
            {
                return context.Faulted(ex);
            }
        }
    }
}
