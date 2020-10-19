using FWTL.Core.Credentials;

namespace FWTL.Database
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