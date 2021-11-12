using System;
using System.Collections.Generic;
using System.Linq;
using FWTL.Common.Aggregates;
using FWTL.Core.Aggregates;
using FWTL.Domain.Accounts.AccountSetup;
using FWTL.Domain.Events;

namespace FWTL.Domain.Accounts
{
    public class AccountAggregate :
        AggregateRoot<AccountAggregate>,
        IApply<AccountCreated>,
        IApply<SessionCreated>,
        IApply<CodeSent>,
        IApply<AccountVerified>,
        IApply<SetupFailed>,
        IApply<AccountDeleted>,
        IApply<InfrastructureGenerated>,
        IApply<InfrastructureTearedDown>,
        IApply<AccountSetupRestarted>
    {
        public AccountAggregate()
        {
        }

        public enum AccountState
        {
            Failed = -1,
            Initialized = 1,
            WithInfrastructure = 2,
            WithSession = 3,
            WaitForCode = 4,
            Ready = 5,
        }

        public string ExternalAccountId { get; set; }

        public Guid OwnerId { get; set; }

        public AccountState State { get; set; }

        public void Apply(AccountCreated @event)
        {
            Id = @event.AccountId;
            ExternalAccountId = @event.ExternalAccountId;
            OwnerId = @event.OwnerId;
            State = AccountState.Initialized;
        }

        public void Apply(SessionCreated @event) => State = AccountState.WithSession;

        public void Apply(CodeSent @event)
        {
            State = AccountState.WaitForCode;
        }

        public void Apply(AccountVerified @event)
        {
            State = AccountState.Ready;
        }

        public void Apply(SetupFailed @event)
        {
            State = AccountState.Failed;
        }

        public void Apply(AccountDeleted @event)
        {
            Delete();
        }

        public void Apply(InfrastructureGenerated @event)
        {
            State = AccountState.WithInfrastructure;
        }

        public void Apply(InfrastructureTearedDown @event)
        {
            State = AccountState.Failed;
        }

        public void Create(Guid accountId, CreateAccount.Command command)
        {
            var accountAdded = new AccountCreated()
            {
                AccountId = accountId,
                ExternalAccountId = command.ExternalAccountId,
                OwnerId = command.UserId,
            };

            AddEvent(accountAdded);
        }

        public void CreateSession()
        {
            AddEvent(new SessionCreated() { AccountId = Id });
        }

        public void Delete(Guid deletedBy)
        {
            var accountDeleted = new AccountDeleted()
            {
                DeletedBy = deletedBy,
                State = State,
                AccountId = Id
            };

            AddEvent(accountDeleted);
        }

        public void FailSetup(IEnumerable<string> errors)
        {
            AddEvent(new SetupFailed()
            {
                AccountId = Id,
                CurrentState = State,
                Errors = errors.ToList()
            });
        }

        public void GenerateInfrastructure()
        {
            AddEvent(new InfrastructureGenerated()
            {
                AccountId = Id
            });
        }

        public bool HasInfrastructure()
        {
            return State >= AccountState.WithInfrastructure;
        }

        public void Reset()
        {
            AddEvent(new AccountSetupRestarted() { AccountId = Id });
        }

        public void SendCode()
        {
            AddEvent(new CodeSent()
            {
                AccountId = Id
            });
        }

        public void TearDownInfrastructure()
        {
            AddEvent(new InfrastructureTearedDown()
            {
                AccountId = Id
            });
        }

        public void Verify()
        {
            AddEvent(new AccountVerified()
            {
                AccountId = Id
            });
        }

        public void Apply(AccountSetupRestarted @event)
        {
            State = AccountState.Initialized;
        }
    }
}