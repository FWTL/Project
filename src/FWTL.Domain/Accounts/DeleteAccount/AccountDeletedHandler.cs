using System;
using System.Threading.Tasks;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Domain.Events;

namespace FWTL.Domain.Accounts.DeleteAccount
{
    public class AccountDeletedHandler : IEventHandler<AccountDeleted>
    {
        private readonly IInfrastructureService _infrastructureService;

        public AccountDeletedHandler(IInfrastructureService infrastructureService)
        {
            _infrastructureService = infrastructureService;
        }

        public async Task HandleAsync(AccountDeleted @event)
        {
            var result = await _infrastructureService.TearDownTelegramApi(@event.AccountId);
            if (!result.IsSuccess)
            {
                throw new Exception(string.Join(",", result.Errors));
            }
        }
    }
}