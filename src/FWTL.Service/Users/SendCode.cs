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
using FWTL.Aggregate;
using FWTL.Domain.Traits;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Domain.Users
{
    public class AddTelegramAccount
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
            private readonly IAuthDatabaseContext _dbAuthDatabaseContext;

            public IList<IEvent> Events => new List<IEvent>();

            public Handler(ITelegramClient telegramClient, IAuthDatabaseContext dbAuthDatabaseContext)
            {
                _telegramClient = telegramClient;
                _dbAuthDatabaseContext = dbAuthDatabaseContext;
            }

            public async Task ExecuteAsync(Command command)
            {
                
                await _telegramClient.SystemService.AddSessionAsync(command.SessionName());
                await _telegramClient.UserService.PhoneLoginAsync(command.SessionName(), command.AccountId);

                bool doesAccountAlreadyExist = await _dbAuthDatabaseContext.TelegramAccount.AnyAsync(ta =>
                    ta.Id == command.AccountId && ta.UserId == command.UserId);

                if (!doesAccountAlreadyExist)
                {
                    await _dbAuthDatabaseContext.TelegramAccount.AddAsync(new TelegramAccount()
                    {
                        Id = command.AccountId,
                        UserId = command.UserId
                    });
                    await _dbAuthDatabaseContext.SaveChangesAsync();
                }
            }
        }

        public class Validator : AppAbstractValidation<Command>
        {
            public Validator()
            {
                RuleFor(x => x.AccountId).Matches(RegexExpressions.ONLY_NUMBERS);
            }
        }
    }
}