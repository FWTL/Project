using System;
using System.Threading.Tasks;
using FWTL.Core.Queries;
using FWTL.Core.Services;

namespace FWTL.Domain.Users
{
    public class GetMe
    {
        public class Query : IQuery
        {
            private Guid CurrentUserId { get; set; }

            public Query()
            {
            }

            public Query(ICurrentUserService currentUserService)
            {
                CurrentUserId = currentUserService.CurrentUserId;
            }
        }

        public class Result
        {
            public string Name { get; set; }

            public string Avatar { get; set; }

            public string TimeZone { get; set; }

            public string PhoneNumber { get; set; }
        }

        public class Handler : IQueryHandler<Query, Result>
        {
            public Handler()
            {
            }

            public Task<Result> HandleAsync(Query query)
            {
                var result = new Result()
                {
                    Name = "Andrzej Golaszewski",
                    Avatar = "https://api.adorable.io/avatars/64/asdasdasds@dasda.aa.png",
                    TimeZone = "",
                    PhoneNumber = ""
                };

                return Task.FromResult(result);
            }
        }
    }
}