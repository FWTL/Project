using FWTL.Core.Credentials;

namespace FWTL.Common.Setup.Credentials
{
    public class RedisCredentials : IConnectionString
    {
        public RedisCredentials(RedisLocalCredentialsBase credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public RedisCredentials(RedisAzureCredentialsBase credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public string ConnectionString { get; private set; }
    }
}