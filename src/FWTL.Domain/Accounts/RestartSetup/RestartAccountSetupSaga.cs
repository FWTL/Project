using System;
using Automatonymous;
using FWTL.Core.Events;
using FWTL.Domain.Accounts.AccountSetup;
using FWTL.Domain.Accounts.DeleteAccount;
using FWTL.Domain.Events;

namespace FWTL.Domain.Accounts.RestartSetup
{
    //https://miro.com/app/board/o9J_lDZlCU8=/
    public class RestartAccountSetupSaga : MassTransitStateMachine<RestartAccountSetupState>
    {
        public State Restart { get; }

        public RestartAccountSetupSaga()
        {
            
        }
    }
}