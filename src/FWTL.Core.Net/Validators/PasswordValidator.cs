using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FWTL.Common.Net.Helpers;
using IdentityServer4.Validation;

namespace FWTL.Common.Net.Validators
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            GrantValidationResultHelpers.Success(context.UserName, "FWTL", new List<Claim>());
            return Task.CompletedTask;
        }
    }
}