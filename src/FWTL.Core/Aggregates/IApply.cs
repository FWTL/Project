using FWTL.Core.Events;

namespace FWTL.Core.Aggregates
{
    public interface IApply<TEvent> where TEvent : IEvent
    {
        void Apply(TEvent @event);
    }
}