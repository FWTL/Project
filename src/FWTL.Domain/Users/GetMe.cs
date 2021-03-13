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
            public Handler()
            {
            }

            public async Task<Result> HandleAsync(Query query)
            {
                return null;
            }
        }
    }
}