using FluentValidation;
using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Database;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.TelegramClient;
using FWTL.TelegramClient.Exceptions;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Fallback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FWTL.Domain.Users
{
    public class DeleteAccount
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

            private static readonly AsyncFallbackPolicy IgnoreBadRequestsPolicy = Policy
                .Handle<TelegramClientException>()
                .FallbackAsync(token => Task.CompletedTask);

            public IList<IEvent> Events => new List<IEvent>();

            public Handler(ITelegramClient telegramClient, IAuthDatabaseContext databaseContext)
            {
                _telegramClient = telegramClient;
                _databaseContext = databaseContext;
            }

            public async Task ExecuteAsync(Command command)
            {
                string sessionName = command.UserId.ToSession(command.PhoneNumber);

                await IgnoreBadRequestsPolicy.ExecuteAsync(() => _telegramClient.UserService.LogoutAsync(sessionName));
                await IgnoreBadRequestsPolicy.ExecuteAsync(() => _telegramClient.SystemService.RemoveSessionAsync(sessionName));
                await IgnoreBadRequestsPolicy.ExecuteAsync(() => _telegramClient.SystemService.UnlinkSessionFileAsync(sessionName));

                var telegramAccount = await _databaseContext.TelegramAccount.Where(ta => ta.UserId == command.UserId && ta.Number == command.PhoneNumber).FirstOrDefaultAsync();
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