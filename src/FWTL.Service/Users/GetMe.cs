using FWTL.Aggregate;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

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
        }

        public class Handler : IQueryHandler<Query, Result>
        {
            private readonly UserManager<User> _userManager;

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public async Task<Result> HandleAsync(Query query)
            {
                var user = await _userManager.FindByIdAsync(query.UserId.ToString());

                return new Result()
                {
                    Id = user.Id,
                    Email = user.Email
                };
            }
        }
    }
}