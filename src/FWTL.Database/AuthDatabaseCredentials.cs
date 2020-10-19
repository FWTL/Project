using FWTL.Core.Credentials;

namespace FWTL.Database
{
    public class AppDatabaseCredentials
    {
        public AppDatabaseCredentials(IConnectionString credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public string ConnectionString { get; }
    }
}