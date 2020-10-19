using FWTL.Core.Commands;
using FWTL.Core.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWTL.Domain.Jobs
{
    public class GetJobsByDialog
    {
        public class Request : IRequest
        {
            public string AccountId { get; set; }
            public string DialogId { get; set; }
        }

        public class Query : IQuery
        {
        }

        public class Result
        {
        }

        public class Handler : IQueryHandler<Query, IReadOnlyList<Result>>
        {
            public Task<IReadOnlyList<Result>> HandleAsync(Query query)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}