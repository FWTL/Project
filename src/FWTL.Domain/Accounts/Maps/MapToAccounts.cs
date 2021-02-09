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

        public async Task SaveAsync(AccountAggregate aggregate)
        {
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