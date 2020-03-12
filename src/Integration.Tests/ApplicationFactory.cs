using System.Net.Http;
using System.Threading.Tasks;
using FWTL.Auth.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Integration.Tests
{
    public class ApplicationFactory
    {
        public async Task<HttpClient> CreateAsync()
        {
            var hostBuilder = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                });

            var host = await hostBuilder.StartAsync();
            return host.GetTestClient();
        }
    }
}