using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.TelegramClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWTL.Domain.Users
{
    public class VerifyAccount
    {
        public class Request : IRequest
        {
            public string PhoneNumber { get; set; }

            public string Code { get; set; }
        }

        public class Command : Request, ICommand
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

            public Handler(ITelegramClient telegramClient)
            {
                _telegramClient = telegramClient;
            }

            public IList<IEvent> Events => new List<IEvent>();

            public async Task ExecuteAsync(Command command)
            {
                string sessionName = command.UserId.ToSession(command.PhoneNumber);
                await _telegramClient.UserService.CompletePhoneLoginAsync(sessionName, command.Code);
            }
        }
    }
}