using FluentValidation;
using FWTL.Common.Helpers;
using FWTL.Core.Validation;

namespace FWTL.Domain.Traits
{
    public class SessionNameTraitValidator : AppAbstractValidation<ISessionNameTrait>
    {
        public SessionNameTraitValidator()
        {
            RuleFor(x => x.AccountId).Matches(RegexExpressions.OnlyNumbers);
        }
    }
}