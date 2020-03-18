using System.Threading.Tasks;
using FWTL.Common.Net.Helpers;
using FWTL.Domain.Users;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;

namespace FWTL.Auth
{
    public class ExternalGrantValidator : IExtensionGrantValidator
    {
        private readonly UserManager<User> _userManager;

        public ExternalGrantValidator(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public string GrantType => "external";

        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var externalToken = context.Request.Raw.Get("externalToken");

            if (string.IsNullOrWhiteSpace(externalToken))
            {
                context.Result = GrantValidationResultHelpers.Error("invalid external token");
                return Task.CompletedTask;
            }

            context.Result = GrantValidationResultHelpers.Success("", "FWTL", null);
            return Task.CompletedTask;
        }
    }
}