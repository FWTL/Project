using FWTL.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace FWTL.Domain.Traits
{
    public interface IPagingTrait
    {
        int Start { get; set; }
        int Limit { get; set; }
    }

    public class PagingTraitValidator : AppAbstractValidation<IPagingTrait>
    {
        public PagingTraitValidator()
        {
            RuleFor(x => x.Start).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Limit).GreaterThan(0);
            RuleFor(x => x.Limit).LessThanOrEqualTo(50);
        }
    }
}
