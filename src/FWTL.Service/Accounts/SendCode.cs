using FluentValidation;
using FWTL.Common.Extensions;
using FWTL.Common.Helpers;
using FWTL.Core.Commands;
using FWTL.Core.Database;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.TelegramClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Domain.Traits;

namespace FWTL.Domain.Users
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

            public Handler(ITelegramClient telegramClient, IAuthDatabaseContext dbAuthDatabaseContext)
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