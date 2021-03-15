using FWTL.Common.Setup.Credentials;
using FWTL.Database.Access;
using Microsoft.Extensions.Configuration;

namespace FWTL.Management.Configuration
{
    public class SolutionConfiguration
    {
        public SolutionConfiguration(IConfiguration configuration)
        {
            AppDatabaseCredentials = new AppDatabaseCredentials(new SqlServerDatabaseCredentials(configuration, "App"));
            HangfireDatabaseCredentials = new HangfireDatabaseCredentials(new SqlServerDatabaseCredentials(configuration, "Hangfire"));
            EventStoreCredentials = new EventStoreCredentials(new EventStoreCredentialsBase(configuration));
            RedisCredentials = new RedisCredentials(new RedisLocalCredentialsBase(configuration));
        }

        public AppDatabaseCredentials AppDatabaseCredentials { get; }

        public HangfireDatabaseCredentials HangfireDatabaseCredentials { get; }

        public EventStoreCredentials EventStoreCredentials { get; }

        public RedisCredentials RedisCredentials { get; }
    }
}
