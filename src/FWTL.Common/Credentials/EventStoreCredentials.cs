using FWTL.Core.Credentials;

namespace FWTL.Common.Credentials
{
    public class EventStoreCredentials : IConnectionString
    {
        public EventStoreCredentials(EventStoreCredentialsBase credentials)
        {
            ConnectionString = credentials.ConnectionString;
        }

        public string ConnectionString { get; private set; }
    }
}