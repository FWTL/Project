using System;
using FWTL.Core.Queries;
using FWTL.Core.Services;

namespace FWTL.Domain.Users
{
    public class GetMe
    {
        public class Query : IQuery
        {
            private Guid CurrentUserId { get; set; }

            public Query(ICurrentUserService currentUserService)
            {
                CurrentUserId = currentUserService.CurrentUser;
            }
        }

        public class Result
        {
        }
    }
}