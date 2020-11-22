using System;
using FWTL.Common.Aggregates;
using FWTL.Core.Aggregates;
using FWTL.Domain.Accounts.AccountSetup;
using FWTL.Events;

namespace FWTL.Domain.Accounts
{
    public class AccountAggregate :
        AggregateRoot<AccountAggregate>,
        IApply<AccountCreated>
    {
        public string ExternalAccountId { get; set; }

        public Guid OwnerId { get; set; }

        public AccountState State { get; set; }

        public enum AccountState
        {
            Initialized = 1,
            WithSession = 2,
            WaitForCode = 3,
            Ready = 4
        }

        public void Apply(AccountCreated @event)
        {
            Id = @event.AccountId;
            ExternalAccountId = @event.ExternalAccountId;
            OwnerId = @event.OwnerId;
            State = AccountState.Initialized;
        }

        public void Create(Guid id, AddAccount.Command command)
        {
            var accountAdded = new AccountCreated()
            {
                AccountId = id,
                ExternalAccountId = command.ExternalAccountId,
                OwnerId = command.UserId,
            };

            AddEvent(accountAdded);
        }

        public override Func<AccountAggregate, string> UniquenessFn { get; } = x => $"{x.OwnerId}:{x.ExternalAccountId}";
    }
}