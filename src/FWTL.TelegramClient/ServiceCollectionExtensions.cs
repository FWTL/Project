using System;
using FWTL.Core.Services.Telegram;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.TelegramClient
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddTelegramClient(this IServiceCollection services, Uri telegramUrl)
        {
            return services.AddHttpClient<ITelegramClient, Client>((service, client) =>
            {
                client.BaseAddress = telegramUrl;
            });
        }
    }
}
