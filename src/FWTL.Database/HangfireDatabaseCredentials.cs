using FWTL.Core.Credentials;

namespace FWTL.Auth.Database
{
    public class HangfireDatabaseCredentials
    {
        public HangfireDatabaseCredentials(IConnectionString credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public string ConnectionString { get; }
    }
}