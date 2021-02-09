using System.Threading.Tasks;

namespace FWTL.Core.Aggregates
{
    public interface IAggregateMap<in TAggregate> where TAggregate : IAggregateRoot
    {
        Task SaveAsync(TAggregate aggregate);
    }
}