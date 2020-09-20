using FWTL.Aggregate;
using FWTL.Core.Commands;
using FWTL.Core.Database;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Domain.Traits;
using FWTL.TelegramClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWTL.Domain.Users
{
    public class AddAccount
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

            public Command(ICurrentUserService currentUserService, IGuidService guidService)
            {
                Id = guidService.New;
                UserId = currentUserService.CurrentUserId;
            }

            public Guid Id { get; set; }

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

                bool doesAccountAlreadyExist = await _dbAuthDatabaseContext.Accounts.AnyAsync(ta =>
                    ta.ExternalId == command.AccountId && ta.UserId == command.UserId);

                if (!doesAccountAlreadyExist)
                {
                    await _dbAuthDatabaseContext.Accounts.AddAsync(new Account()
                    {
                        Id = command.Id,
                        ExternalId = command.AccountId,
                        UserId = command.UserId
                    });
                    await _dbAuthDatabaseContext.SaveChangesAsync();
                }
            }
        }
    }
}