using FluentValidation;
using FWTL.Core.Aggregates;
using FWTL.Core.Events;

namespace FWTL.Common.Specification
{
    public interface ISpecificationFor<TAggregate, TEvent> where TEvent : IEvent where TAggregate : IAggregateRoot
    {
        IValidator<TAggregate> Apply(TEvent @event);
    }
}