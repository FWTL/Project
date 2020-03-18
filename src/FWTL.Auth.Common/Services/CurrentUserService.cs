using System;
using FWTL.Core.Services;

namespace FWTL.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService()
        {
            CurrentUser = 48727842501;
        }

        public long CurrentUser { get; }
    }
}