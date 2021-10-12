using System;
using FWTL.Core.Services;

namespace FWTL.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService()
        {
        }

        public Guid CurrentUserId => Guid.Parse("fe511212-15ae-404a-b3df-bb7d300c1283");
    }
}