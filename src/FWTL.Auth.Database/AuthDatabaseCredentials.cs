using FWTL.Core.Credentials;

namespace FWTL.Auth.Database
{
    public class AuthDatabaseCredentials
    {
        public AuthDatabaseCredentials(IConnectionString credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public string ConnectionString { get; }
    }
}