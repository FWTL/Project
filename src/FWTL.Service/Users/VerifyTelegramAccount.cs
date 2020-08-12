using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.TelegramClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Aggragate;
using FWTL.Core.Database;

namespace FWTL.Domain.Users
{
    public class VerifyTelegramAccount
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
            private readonly IAuthDatabaseContext _dbAuthDatabaseContext;

            public Handler(ITelegramClient telegramClient, IAuthDatabaseContext dbAuthDatabaseContext)
            {
                _telegramClient = telegramClient;
                _dbAuthDatabaseContext = dbAuthDatabaseContext;
            }

            public IList<IEvent> Events => new List<IEvent>();

            public async Task ExecuteAsync(Command command)
            {
                string sessionName = command.UserId + "/" + command.PhoneNumber;
                await _telegramClient.UserService.CompletePhoneLoginAsync(sessionName, command.Code);

                await _dbAuthDatabaseContext.TelegramAccount.AddAsync(new TelegramAccount()
                {
                    Number = command.PhoneNumber,
                    UserId = command.UserId
                });
            }
        }
    }
}