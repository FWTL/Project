using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWTL.Domain.Users
{
    public class VerifyTelegramAccount
    {
        public class Request : IRequest
        {
            public string Code { get; set; }
        }

        public class Command : Request, ICommand
        {
            public Command(ICurrentUserService currentUserService)
            {
                UserId = currentUserService.CurrentUserId;
            }

            public Guid UserId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            public IList<IEvent> Events => throw new NotImplementedException();

            public Task ExecuteAsync(Command command)
            {
                throw new NotImplementedException();
            }
        }
    }
}