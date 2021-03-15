using FWTL.Common.Extensions;
using FWTL.Common.Setup.Extensions;
using FWTL.Core.Credentials;
using Microsoft.Extensions.Configuration;

namespace FWTL.Common.Setup.Credentials
{
    public sealed class EventStoreCredentialsBase : IConnectionString
    {
        public EventStoreCredentialsBase(IConfiguration configuration)
        {
            User = configuration.GetNotNullOrEmpty("EventStore:User");
            Password = configuration.GetNotNullOrEmpty("EventStore:Password");
            Ip = configuration.GetNotNullOrEmpty("EventStore:Ip");
            Port = configuration.GetNotNullOrEmpty("EventStore:Port").To<int>();
        }

        public string Ip { get; }

        public string Password { get; }

        public int Port { get; }

        public string User { get; }

        public string ConnectionString => $"tcp://{User}:{Password}@{Ip}:{Port}";
    }
}