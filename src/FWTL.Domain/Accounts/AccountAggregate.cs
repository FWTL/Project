using System;
using System.Collections.Generic;
using System.Linq;
using FWTL.Common.Aggregates;
using FWTL.Common.Exceptions;
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
        IApply<SessionRemoved>
    {
        public AccountAggregate()
        {
        }

        public enum AccountState
        {
            Failed = -1,
            Initialized = 1,
            WithSession = 2,
            WaitForCode = 3,
            Ready = 4
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
            SoftDelete();
        }

        public void Apply(SessionRemoved @event)
        {
            State = AccountState.Initialized;
        }

        public void Create(Guid accountId, AddAccount.Command command)
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

        public void RemoveSession()
        {
            AddEvent(new SessionRemoved() { AccountId = Id });
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

        public void SessionNotFound()
        {
            AddEvent(new SessionNotFound() { AccountId = Id });
        }

        public void TryToCreateSession()
        {
            if (State != AccountState.Initialized)
            {
                throw new AppValidationException(nameof(State), $"Account is not in Initialized state");
            }
        }

        public void TryToSendCode()
        {
            if (State != AccountState.WithSession)
            {
                throw new AppValidationException(nameof(State), $"Account is not in WithSession state");
            }
        }

        public void TryToVerify()
        {
            if (State != AccountState.WaitForCode)
            {
                throw new AppValidationException(nameof(State), $"Account is not in WaitForCode state");
            }
        }

        public void UnlinkSession()
        {
            AddEvent(new SessionUnlinked() { AccountId = Id });
        }

        public void Verify()
        {
            AddEvent(new AccountVerified()
            {
                AccountId = Id
            });
        }
    }
}