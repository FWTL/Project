using FluentValidation;
using FWTL.Common.Validators;

namespace FWTL.Domain.Traits
{
    public class PagingTraitValidator : AppAbstractValidation<IPagingTrait>
    {
        public PagingTraitValidator()
        {
            RuleFor(x => x.Start).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Limit).LessThanOrEqualTo(50).GreaterThan(0);
        }
    }
}