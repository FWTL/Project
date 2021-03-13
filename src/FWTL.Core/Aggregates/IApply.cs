using FWTL.Core.Events;

namespace FWTL.Core.Aggregates
{
    public interface IApply<in TEvent> where TEvent : IEvent
    {
        void Apply(TEvent @event);
    }
}