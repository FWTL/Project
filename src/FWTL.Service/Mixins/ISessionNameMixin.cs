using FWTL.Common.Extensions;
using System;

namespace FWTL.Domain.Mixins
{
    internal interface ISessionNameMixin
    {
        string AccountId { get; }

        Guid UserId { get; }

        string SessionName => UserId.ToSession(AccountId);
    }
}