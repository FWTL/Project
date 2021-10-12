using FWTL.Common.Extensions;
using FWTL.Common.Setup.Extensions;
using FWTL.Core.Credentials;
using Microsoft.Extensions.Configuration;

namespace FWTL.Common.Setup.Credentials
{
    public sealed class RedisAzureCredentialsBase : IConnectionString
    {
        private readonly bool _allowAdmin;

        private readonly bool _isSsl;

        private readonly string _name;

        private readonly string _password;

        private readonly int _port;

        public RedisAzureCredentialsBase(IConfiguration configuration)
        {
            _name = configuration.Get("Redis:Name");
            _password = configuration.Get("Redis:Password");
            _port = configuration.Get("Redis:Port").To<int>();

            _isSsl = true;
            _allowAdmin = true;
        }

        public string ConnectionString =>
            $"{_name}.redis.cache.windows.net:{_port},password={_password},ssl={_isSsl},abortConnect=False,allowAdmin={_allowAdmin}";
    }
}