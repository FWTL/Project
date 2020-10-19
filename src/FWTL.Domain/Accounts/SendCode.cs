using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Core.Database;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Domain.Traits;
using FWTL.TelegramClient;

namespace FWTL.Domain.Accounts
{
    public class SendCode
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
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly ITelegramClient _telegramClient;

            public IList<IEvent> Events => new List<IEvent>();

            public Handler(ITelegramClient telegramClient, DatabaseContext dbAuthDatabaseContext)
            {
                _telegramClient = telegramClient;
            }

            public async Task ExecuteAsync(Command command)
            {
                await _telegramClient.UserService.PhoneLoginAsync(command.SessionName(), command.AccountId);
            }
        }
    }
}