using FWTL.Core.Queries;
using FWTL.Core.Services;
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

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string TimeZoneName { get; set; }

            public string TimeZoneId { get; set; }
        }

        public class Handler : IQueryHandler<Query, Result>
        {
            public Task<Result> HandleAsync(Query query)
            {
                return Task.FromResult(new Result()
                {
                    FirstName = "Andrzej",
                    LastName = "Golaszewski",
                    Id = query.UserId,
                    TimeZoneId = null,
                    TimeZoneName = null
                });
            }
        }
    }
}