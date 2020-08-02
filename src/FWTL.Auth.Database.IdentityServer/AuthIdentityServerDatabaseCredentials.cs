using FWTL.Core.Credentials;

namespace FWTL.Auth.Database.IdentityServer
{
    public class AuthIdentityServerDatabaseCredentials
    {
        public AuthIdentityServerDatabaseCredentials(IConnectionString credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public string ConnectionString { get; }
    }
}