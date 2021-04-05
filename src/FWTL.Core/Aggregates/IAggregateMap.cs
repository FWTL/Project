using System.Threading.Tasks;

namespace FWTL.Core.Aggregates
{
    public interface IAggregateMap<in TAggregate> where TAggregate : IAggregateRoot
    {
        Task CreateAsync(TAggregate aggregate);

        Task UpdateAsync(TAggregate aggregate);

        Task DeleteAsync(TAggregate aggregate);
    }
}