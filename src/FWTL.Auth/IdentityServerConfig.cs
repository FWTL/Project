using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FWTL.Auth
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api", "FWTL")
            };
        }

        public static IEnumerable<Client> GetClients(IConfigurationRoot appConfiguration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "FWTL",
                    AllowedGrantTypes =  { GrantType.ResourceOwnerPassword },
                    RequireClientSecret = false,
                    AllowedScopes = new List<string> {"api"},
                    AccessTokenLifetime = 60 * 60 * 10,
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            };
        }
    }
}