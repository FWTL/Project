using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Aggregate;
using FWTL.Common.Extensions;
using FWTL.Core.Database;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.TelegramClient;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Domain.Accounts
{
    public class GetAccounts
    {
        public class Query : IQuery
        {
            public Query()
            {
            }

            public Query(ICurrentUserService currentUserService)
            {
                UserId = currentUserService.CurrentUserId;
            }

            public Guid UserId { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }

            public string ExternalId { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string UserName { get; set; }

            public bool IsLogged { get; set; }
        }

        public class Handler : IQueryHandler<Query, IReadOnlyList<Result>>
        {
            private readonly ITelegramClient _telegramClient;
            private readonly DatabaseContext _dbAuthDatabaseContext;

            public Handler(ITelegramClient telegramClient, DatabaseContext dbAuthDatabaseContext)
            {
                _telegramClient = telegramClient;
                _dbAuthDatabaseContext = dbAuthDatabaseContext;
            }

            public async Task<IReadOnlyList<Result>> HandleAsync(Query query)
            {
                var accounts = await _dbAuthDatabaseContext.Accounts.Where(ta => ta.UserId == query.UserId).ToListAsync();

                var telegramAccounts = new List<Result>();
                foreach (Account account in accounts)
                {
                    string sessionName = query.UserId.ToSession(account.ExternalId);
                    var result = await _telegramClient.UserService.GetSelfAsync(sessionName);

                    if (result.IsNotNull())
                    {
                        telegramAccounts.Add(new Result()
                        {
                            Id = account.Id,
                            FirstName = result.Firstname,
                            LastName = result.Lastname,
                            ExternalId = account.ExternalId,
                            UserName = result.Username,
                            IsLogged = true,
                        });
                    }
                    else
                    {
                        telegramAccounts.Add(new Result()
                        {
                            Id = account.Id,
                            ExternalId = account.ExternalId,
                            IsLogged = false,
                        });
                    }
                }

                return telegramAccounts;
            }
        }
    }
}