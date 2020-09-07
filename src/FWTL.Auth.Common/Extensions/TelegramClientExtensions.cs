using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using FWTL.TelegramClient;
using Microsoft.AspNetCore.Identity;

namespace FWTL.Common.Extensions
{
    public static class TelegramClientExtensions
    {
        public static IEnumerable<ValidationFailure> GetErrors<TResponse>(this ResponseWrapper<TResponse> @this)
        {
            var name = typeof(TResponse).Name;
            return @this.Errors.Select(error => new ValidationFailure(name, error.Message));
        }

        public static IEnumerable<ValidationFailure> GetErrors(this IdentityResult @this, string propertyName = nameof(IdentityResult))
        {
            return @this.Errors.Select(error => new ValidationFailure(propertyName, error.Description));
        }

        public static string ToSession(this Guid userId, string accountId)
        {
            return userId + "/" + accountId;
        }
    }
}