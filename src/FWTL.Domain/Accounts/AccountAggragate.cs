using System;
using FWTL.Common.Aggregates;
using FWTL.Core.Aggregates;
using FWTL.Domain.Accounts.AccountSetup;
using FWTL.Events;

namespace FWTL.Domain.Accounts
{
    public class AccountAggregate :
        AggregateRoot<AccountAggregate>,
        IApply<AccountCreated>,
        IApply<SessionCreated>,
        IApply<CodeSent>
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

        public AccountAggregate()
        {
        }

        public AccountAggregate(Guid ownerId, string externalAccountId)
        {
            OwnerId = ownerId;
            ExternalAccountId = externalAccountId;
        }

        public void Apply(AccountCreated @event)
        {
            ExternalAccountId = @event.ExternalAccountId;
            OwnerId = @event.OwnerId;
            State = AccountState.Initialized;
        }

        public void SendCode()
        {
            AddEvent(new CodeSent());
        }

        public void Create(AddAccount.Command command)
        {
            var accountAdded = new AccountCreated()
            {
                ExternalAccountId = command.ExternalAccountId,
                OwnerId = command.UserId,
            };

            AddEvent(accountAdded);
        }

        public void CreateSession()
        {
            AddEvent(new SessionCreated() { AccountId = Id });
        }

        public void Apply(SessionCreated @event)
        {
            State = AccountState.WithSession;
        }

        public void Apply(CodeSent @event)
        {
            State = AccountState.WaitForCode;
        }

        public override Func<AccountAggregate, string> UniquenessFn { get; } = x => $"{x.OwnerId}:{x.ExternalAccountId}";
    }
}