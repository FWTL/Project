namespace FWTL.Core.Aggregates
{
    public interface IAggregateMap<TAggregate> where TAggregate : IAggregateRoot
    {
        void Save(TAggregate aggregate);
    }
}