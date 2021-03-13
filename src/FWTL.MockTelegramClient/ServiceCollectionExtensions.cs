using FWTL.Core.Services.Telegram;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.MockTelegramClient
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMockTelegramClient(this IServiceCollection services)
        {
            services.AddScoped<ITelegramClient, MockClient>();
        }
    }
}
