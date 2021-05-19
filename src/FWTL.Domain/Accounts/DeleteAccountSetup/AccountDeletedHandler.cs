using System.Threading.Tasks;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Domain._Extensions;
using FWTL.Events;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;

namespace FWTL.Domain.Accounts.DeleteAccountSetup
{
    public class AccountDeletedHandler : IEventHandler<AccountDeleted>
    {
        private readonly IGuidService _guidService;
        private readonly IBus _bus;

        public AccountDeletedHandler(IGuidService guidService, IBus bus)
        {
            _guidService = guidService;
            _bus = bus;
        }

        public async Task HandleAsync(AccountDeleted @event)
        {
            var builder = new RoutingSlipBuilder(_guidService.New);
            builder.AddActivity(new Logout.Logout.Command() { AccountId = @event.AccountId });
            builder.AddActivity(new Logout.Logout2.Command() { AccountId = @event.AccountId });
            RoutingSlip slip = builder.Build();
            await _bus.Execute(slip);
        }
    }
}