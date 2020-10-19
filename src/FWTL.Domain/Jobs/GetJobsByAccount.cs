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
            public Task<IReadOnlyList<Result>> HandleAsync(Query query)
            {
                throw new NotImplementedException();
            }
        }
    }
}