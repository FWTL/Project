using Automatonymous;

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