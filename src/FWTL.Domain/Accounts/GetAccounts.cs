using FWTL.Core.Queries;
using FWTL.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            public Task<IReadOnlyList<Result>> HandleAsync(Query query)
            {
                throw new NotImplementedException();
            }
        }
    }
}