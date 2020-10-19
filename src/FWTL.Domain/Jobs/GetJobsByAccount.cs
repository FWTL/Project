using FWTL.Core.Commands;
using FWTL.Core.Database;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.Domain.Traits;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FWTL.Domain.Jobs
{
    public class GetJobsByAccount
    {
        public class Request : IRequest
        {
            public string AccountId { get; set; }
        }

        public class Query : Request, IQuery, ISessionNameTrait
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
        }

        public class Handler : IQueryHandler<Query, IReadOnlyList<Result>>
        {
            private readonly DatabaseContext _dbContext;

            public Handler(DatabaseContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<IReadOnlyList<Result>> HandleAsync(Query query)
            {
                List<Result> result = await _dbContext.Accounts
                .Where(ta => ta.ExternalId == query.AccountId)
                .Where(ta => ta.UserId == query.UserId)
                .Include(ta => ta.AccountJobs)
                .ThenInclude(taj => taj.Job).Select(j => new Result()
                {
                    Id = j.Id
                }).ToListAsync();

                return result;
            }
        }
    }
}