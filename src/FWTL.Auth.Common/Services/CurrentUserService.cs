using System;
using FWTL.Core.Services;

namespace FWTL.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService()
        {
            CurrentUser = Guid.Parse("8630b2a6-8fd0-4d63-afa6-a0e2ecef5bb9");
        }

        public Guid CurrentUser { get; }
    }
}