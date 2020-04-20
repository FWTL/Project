using System;
using FWTL.Core.Services;

namespace FWTL.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService()
        {
            CurrentUser = Guid.Parse("9b96f1f9-ed7d-40b1-87e9-8373280afdb5");
        }

        public Guid CurrentUser { get; }
    }
}