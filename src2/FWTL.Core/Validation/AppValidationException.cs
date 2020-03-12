using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace FWTL.Core.Validation
{
    public class AppValidationException : ValidationException
    {
        public AppValidationException(IEnumerable<ValidationFailure> errors) : base(errors.ToList())
        {
        }

        public AppValidationException(string property, string message) : base(new List<ValidationFailure>
            {new ValidationFailure(property, message)})
        {
        }
    }
}