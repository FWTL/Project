using System;
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
            RedisCredentials = new RedisCredentials(new RedisLocalCredentialsBase(configuration));
            RabbitMqCredentials = new RabbitMqCredentials(configuration);
            SeqUrl = new Uri(configuration.Get("Seq:Url"));
            TelegramUrl = new Uri(configuration.Get("Telegram:Url"));
            EventStoreUrl = new Uri(configuration.Get("EventStore:Url"));
        }

        public RabbitMqCredentials RabbitMqCredentials { get; }

        public AppDatabaseCredentials AppDatabaseCredentials { get; }

        public HangfireDatabaseCredentials HangfireDatabaseCredentials { get; }

        public RedisCredentials RedisCredentials { get; }

        public Uri SeqUrl { get; }

        public Uri TelegramUrl { get; }

        public Uri EventStoreUrl { get; }
    }
}