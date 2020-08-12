using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.TelegramClient;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using FWTL.Aggragate;

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

            public string UserName { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Email { get; set; }
        }

        public class Handler : IQueryHandler<Query, Result>
        {
            private readonly UserManager<User> _userManager;
            private readonly ITelegramClient _telegramClient;

            public Handler(UserManager<User> userManager, ITelegramClient telegramClient)
            {
                _userManager = userManager;
                _telegramClient = telegramClient;
            }

            public async Task<Result> HandleAsync(Query query)
            {
                var user = await _userManager.FindByIdAsync(query.UserId.ToString());
                var telegramUser = await _telegramClient.UserService.GetSelfAsync(query.UserId.ToString());

                return new Result()
                {
                    Email = user.Email,
                    FirstName = telegramUser.FirstName,
                    Id = user.Id,
                    LastName = telegramUser.LastName,
                    UserName = telegramUser.UserName
                };
            }
        }
    }
}