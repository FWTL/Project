using System;
using FWTL.Common.Extensions;

namespace FWTL.Domain.Traits
{
    public interface ISessionNameTrait
    {
        string AccountId { get; }

        Guid UserId { get; }

        public string SessionName => UserId.ToSession(AccountId);
    }
}