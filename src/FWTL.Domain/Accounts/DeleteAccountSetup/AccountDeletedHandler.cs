using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FWTL.Core.Events;
using FWTL.Events;

namespace FWTL.Domain.Accounts.DeleteAccountSetup
{
    public class AccountDeletedHandler : IEventHandler<AccountDeleted>
    {
        public Task HandleAsync(AccountDeleted @event)
        {
            throw new NotImplementedException();
        }
    }
}
