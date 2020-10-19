using System.Threading.Tasks;

namespace FWTL.Core.Aggregates
{
    public interface IAggregateDbStore
    {
        Task SaveAsync(IAggregateRoot aggregate);
    }
}