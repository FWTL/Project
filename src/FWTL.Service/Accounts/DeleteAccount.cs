using FluentValidation;
using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Database;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.TelegramClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Domain.Traits;

namespace FWTL.Domain.Users
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
            private readonly IAuthDatabaseContext _databaseContext;

            public IList<IEvent> Events => new List<IEvent>();

            public Handler(ITelegramClient telegramClient, IAuthDatabaseContext databaseContext)
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