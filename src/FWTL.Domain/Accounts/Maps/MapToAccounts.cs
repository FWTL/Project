using System.Linq;
using System.Threading.Tasks;
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

        public async Task<bool> ProbeAsync(AccountAggregate aggregate)
        {
            return await _databaseContext.Accounts.AnyAsync(account => account.Id == aggregate.Id);
        }

        public async Task CreateAsync(AccountAggregate aggregate)
        {
            await _databaseContext.Accounts.AddAsync(new Account()
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
            _databaseContext.Accounts.Remove(new Account()
            {
                Id = aggregate.Id
            });
            await _databaseContext.SaveChangesAsync();
        }

        public async Task SaveAsync(AccountAggregate aggregate)
        {
            if (await _databaseContext.Accounts.AnyAsync(account => account.Id == aggregate.Id))
            {
                return;
            }

            await _databaseContext.Accounts.AddAsync(new Account()
            {
                Id = aggregate.Id,
                ExternalAccountId = aggregate.ExternalAccountId,
                OwnerId = aggregate.OwnerId
            });

            await _databaseContext.SaveChangesAsync();
        }
    }
}