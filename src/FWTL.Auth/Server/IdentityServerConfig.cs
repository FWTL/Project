using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace FWTL.Auth.Server
{
    public class IdentityServerConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api", "FWTL")
            };
        }

        public static IEnumerable<IdentityServer4.Models.Client> GetClients(IConfigurationRoot appConfiguration)
        {
            return new List<IdentityServer4.Models.Client>()
            {
                new IdentityServer4.Models.Client()
                {
                    ClientId = "FWTL",
                    AllowedGrantTypes = new List<string> { "external" },
                    RequireClientSecret = false,
                    AllowedScopes = new List<string> { "api" },
                    AccessTokenLifetime = 60 * 60,
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                }
            };
        }
    }
}