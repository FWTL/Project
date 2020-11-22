using System;
using FWTL.Core.Commands;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class CreateSession
    {
        public class Command : ICommand
        {
            public Command()
            {
            }

            public Guid AccountId { get; set; }
            public Guid CorrelationId { get; set; }
        }
    }
}