using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Common.Extensions;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Database;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.Domain.Traits;
using FWTL.TelegramClient;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Domain.Accounts
{
    public class DeleteAccount
    {
        public class Request : IRequest
        {
            public string AccountId { get; set; }
        }

        public class Command : Request, ICommand, ISessionNameTrait
        {
            public Command()
            {
            }

            public Command(ICurrentUserService currentUserService)
            {
                UserId = currentUserService.CurrentUserId;
            }

            public Guid UserId { get; set; }
            public Guid CorrelationId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            public Task<IAggregateRoot> ExecuteAsync(Command command)
            {
                throw new NotImplementedException();
            }
        }
    }
}