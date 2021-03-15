using System;
using Microsoft.Extensions.Configuration;

namespace FWTL.Common.Setup.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetNotNullOrEmpty(this IConfiguration configuration, string key)
        {
            string result = configuration[key];
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new ArgumentNullException($"{key} is null");
            }

            return result;
        }
    }
}
