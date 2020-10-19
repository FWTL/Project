using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Common.Extensions;
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
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly ITelegramClient _telegramClient;
            private readonly DatabaseContext _databaseContext;

            public IList<IEvent> Events => new List<IEvent>();

            public Handler(ITelegramClient telegramClient, DatabaseContext databaseContext)
            {
                _telegramClient = telegramClient;
                _databaseContext = databaseContext;
            }

            public async Task ExecuteAsync(Command command)
            {
               
                await _telegramClient.UserService.LogoutAsync(command.SessionName());
                await _telegramClient.SystemService.RemoveSessionAsync(command.SessionName());
                //_telegramClient.SystemService.UnlinkSessionFileAsync(sessionName); // doesn't work

                var telegramAccount = await _databaseContext.Accounts.Where(ta => ta.UserId == command.UserId && ta.ExternalId == command.AccountId).FirstOrDefaultAsync();
                if (telegramAccount.IsNull())
                {
                    throw new AppValidationException(nameof(Command.AccountId), "Telegram account not found");
                }

                _databaseContext.Accounts.Remove(telegramAccount);
                await _databaseContext.SaveChangesAsync();
            }
        }
    }
}