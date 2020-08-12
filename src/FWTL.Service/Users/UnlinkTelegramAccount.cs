using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.TelegramClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FWTL.Aggragate;
using FWTL.Core.Database;
using Microsoft.EntityFrameworkCore;
using FWTL.Core.Validation;

namespace FWTL.Domain.Users
{
    public class UnlinkTelegramAccount
    {
        public class Request : IRequest
        {
            public string PhoneNumber { get; set; }
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
            private readonly IAuthDatabaseContext _databaseContext;

            public IList<IEvent> Events => new List<IEvent>();

            public Handler(ITelegramClient telegramClient, IAuthDatabaseContext databaseContext)
            {
                _telegramClient = telegramClient;
                _databaseContext = databaseContext;
            }

            public async Task ExecuteAsync(Command command)
            {
                string sessionName = command.UserId.ToSession(command.PhoneNumber);

                await _telegramClient.SystemService.RemoveSession(sessionName);
                var telegramAccount = await _databaseContext.TelegramAccount.Where(ta =>  ta.UserId == command.UserId && ta.Number == command.PhoneNumber).FirstOrDefaultAsync();
                if (telegramAccount.IsNull())
                {
                    throw new AppValidationException(nameof(Command.PhoneNumber), "Telegram account not found");
                }

                _databaseContext.TelegramAccount.Remove(telegramAccount);
                await _databaseContext.SaveChangesAsync();
            }
        }
    }
}