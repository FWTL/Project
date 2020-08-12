using System;
using System.Collections.Generic;

namespace FWTL.Core.Services
{
    public interface ICurrentUserService
    {
        Guid CurrentUserId { get; }

        string SessionName(string phoneNumber);
    }
}