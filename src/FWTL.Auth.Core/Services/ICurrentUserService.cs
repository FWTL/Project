using System;

namespace FWTL.Core.Services
{
    public interface ICurrentUserService
    {
        Guid CurrentUser { get; }
    }
}