using FWTL.Common.Setup.Credentials;
using FWTL.Common.Setup.Extensions;
using FWTL.Database.Access;
using FWTL.RabbitMq;
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
            RabbitMqCredentials = new RabbitMqCredentials(configuration);
            SeqUrl = configuration.GetNotNullOrEmpty("Seq:Url");
            TelegramUrl = configuration.GetNotNullOrEmpty("Telegram:Url");
        }
        public RabbitMqCredentials RabbitMqCredentials { get; }

        public AppDatabaseCredentials AppDatabaseCredentials { get; }

        public HangfireDatabaseCredentials HangfireDatabaseCredentials { get; }

        public EventStoreCredentials EventStoreCredentials { get; }

        public RedisCredentials RedisCredentials { get; }

        public string SeqUrl { get; }
        public string TelegramUrl { get; }
    }
}
