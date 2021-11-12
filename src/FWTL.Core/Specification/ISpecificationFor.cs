using FluentValidation;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Events;

namespace FWTL.Core.Specification
{
    public interface ISpecificationForEvent<in TAggregate, in TEvent> where TEvent : IEvent where TAggregate : IAggregateRoot
    {
        IValidator<TAggregate> Apply(TEvent @event);
    }

    public interface ISpecificationForCommand<in TAggregate, in TCommand> where TCommand : ICommand where TAggregate : IAggregateRoot
    {
        IValidator<TAggregate> Apply(TCommand command);
    }
}