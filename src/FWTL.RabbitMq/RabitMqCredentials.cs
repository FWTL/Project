using FWTL.Common.Setup.Extensions;
using Microsoft.Extensions.Configuration;

namespace FWTL.RabbitMq
{
    public class RabbitMqCredentials
    {
        public RabbitMqCredentials(IConfiguration configuration)
        {
            UserName = configuration.GetNotNullOrEmpty("RabbitMq:UserName");
            Password = configuration.GetNotNullOrEmpty("RabbitMq:Password");
            Url = configuration.GetNotNullOrEmpty("RabbitMq:Url");
        }

        public string Password { get; }

        public string Url { get; }

        public string UserName { get; }
    }
}
