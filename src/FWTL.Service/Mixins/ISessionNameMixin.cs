using FluentValidation;
using FWTL.Common.Extensions;
using FWTL.Common.Helpers;
using FWTL.Core.Validation;
using System;

namespace FWTL.Domain.Mixins
{
    public interface ISessionNameMixin
    {
        string AccountId { get; }

        Guid UserId { get; }

        public string SessionName => UserId.ToSession(AccountId);
    }

    public class SessionNameMixinValidator : AppAbstractValidation<ISessionNameMixin>
    {
        public SessionNameMixinValidator()
        {
            RuleFor(x => x.AccountId).Matches(RegexExpressions.ONLY_NUMBERS);
        }
    }

    public static class SessionNameMixinExtensions
    {
        public static string SessionName(this ISessionNameMixin @that)
        {
            return @that.SessionName;
        }
    }
}