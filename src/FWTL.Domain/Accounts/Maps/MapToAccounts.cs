using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using FWTL.Common.Extensions;
using FWTL.Core.Aggregates;
using FWTL.Database.Access;
using FWTL.Database.Access.Entities;

namespace FWTL.Domain.Accounts.Maps
{
    public class MapToAccounts : IAggregateMap<AccountAggregate>
    {
        private readonly IDatabaseContext _databaseContext;

        public MapToAccounts(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task CreateAsync(AccountAggregate aggregate)
        {
            await _databaseContext.Connection.InsertAsync(new Account()
            {
                Id = aggregate.Id,
                ExternalAccountId = aggregate.ExternalAccountId,
                OwnerId = aggregate.OwnerId
            });
        }

        public Task UpdateAsync(AccountAggregate aggregate)
        {
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(AccountAggregate aggregate)
        {
            await _databaseContext.Connection.DeleteAsync(new Account()
            {
                Id = aggregate.Id
            });
        }

        public async Task<bool> ProbeAsync(AccountAggregate aggregate)
        {
            if (aggregate.IsNull())
            {
                return false;
            }

            return await _databaseContext.Accounts.AnyAsync(x => x.Id == aggregate.Id);
        }
    }
}