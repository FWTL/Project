using FWTL.Aggragate;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.TelegramClient;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Common.Extensions;
using FWTL.Core.Database;

namespace FWTL.Domain.Users
{
    public class GetMe
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

            public string Email { get; set; }

            public List<TelegramAccount> TelegramAccounts { get; set; }

            public class TelegramAccount
            {
                public string Number { get; set; }

                public string FirstName { get; set; }

                public string LastName { get; set; }

                public string UserName { get; set; }
            }
        }

        public class Handler : IQueryHandler<Query, Result>
        {
            private readonly UserManager<User> _userManager;
            private readonly ITelegramClient _telegramClient;
            private readonly IAuthDatabaseContext _dbAuthDatabaseContext;

            public Handler(UserManager<User> userManager, ITelegramClient telegramClient, IAuthDatabaseContext dbAuthDatabaseContext)
            {
                _userManager = userManager;
                _telegramClient = telegramClient;
                _dbAuthDatabaseContext = dbAuthDatabaseContext;
            }

            public async Task<Result> HandleAsync(Query query)
            {
                var user = await _userManager.FindByIdAsync(query.UserId.ToString());
                var accounts = _dbAuthDatabaseContext.TelegramAccount.Where(ta => ta.UserId == query.UserId)
                    .Select(ta => ta.Number).ToList();

                var telegramAccounts = new List<Result.TelegramAccount>();
                foreach (string account in accounts)
                {
                    string sessionName = query.UserId.ToSession(account);
                    var result = await _telegramClient.UserService.GetSelfAsync(sessionName);

                    telegramAccounts.Add(new Result.TelegramAccount()
                    {
                        FirstName = result.FirstName,
                        LastName = result.LastName,
                        Number = account,
                        UserName = result.UserName
                    });
                }

                return new Result()
                {
                    Id = user.Id,
                    Email = user.Email,
                    TelegramAccounts = telegramAccounts
                };
            }
        }
    }
}