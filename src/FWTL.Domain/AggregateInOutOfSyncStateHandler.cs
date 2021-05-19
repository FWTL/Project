using System.Threading.Tasks;
using FWTL.Common.Extensions;
using FWTL.Core.Aggregates;
using FWTL.Core.Events;
using FWTL.Domain.Accounts;
using FWTL.Events;

namespace FWTL.Domain
{
    public class AggregateInOutOfSyncStateHandler : IEventHandler<AggregateInOutOfSyncState>
    {
        private readonly IAggregateStore _aggregateStore;
        private readonly IAggregateMap<AccountAggregate> _map;

        public AggregateInOutOfSyncStateHandler(IAggregateStore aggregateStore, IAggregateMap<AccountAggregate> map)
        {
            _aggregateStore = aggregateStore;
            _map = map;
        }

        public async Task HandleAsync(AggregateInOutOfSyncState @event)
        {
            AccountAggregate aggregate = await _aggregateStore.GetByIdOrDefaultAsync<AccountAggregate>(@event.AggregateId);
            var doesExist = await _map.ProbeAsync(aggregate);

            if (doesExist && aggregate.IsNull())
            {
                await _map.DeleteAsync(new AccountAggregate() { Id = @event.AggregateId });
                return;
            }

            if (!doesExist && aggregate.Version >= 0)
            {
                await _map.CreateAsync(aggregate);
            }

            if (@event.Version < aggregate.Version)
            {
                return;
            }

            if (doesExist && aggregate.Version >= 0)
            {
                await _map.UpdateAsync(aggregate);
            }
        }
    }
}
