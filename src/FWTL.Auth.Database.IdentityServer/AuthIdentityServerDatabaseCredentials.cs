using FWTL.Common.Credentials;

namespace FWTL.Auth.Database.IdentityServer
{
    public class AuthIdentityServerDatabaseCredentials
    {
        public AuthIdentityServerDatabaseCredentials(LocalDatabaseCredentials credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public AuthIdentityServerDatabaseCredentials(SqlServerDatabaseCredentials credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public string ConnectionString { get; }
    }
}