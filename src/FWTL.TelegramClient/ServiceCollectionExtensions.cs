using System;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Common.Exceptions;
using FWTL.Core.Services.Telegram;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.TelegramClient
{
    public static class ServiceCollectionExtensions
    {
        public static async Task<TResponse> ResponseAsync<TResponse>(this Task<ResponseWrapper<TResponse>> response)
        {
            var awaitedResponse = await response;
            if (awaitedResponse.IsSuccess)
            {
                return awaitedResponse.Response;
            }

            if (awaitedResponse.Errors.All(e => e.Message.Contains("No sessions available") || e.Message.Contains("Session not found")))
            {
                throw new TelegramSessionNotFoundException();
            }

            throw new TelegramClientException(awaitedResponse.Errors);
        }

        public static TResponse Response<TResponse>(ResponseWrapper<TResponse> response)
        {
            if (response.IsSuccess)
            {
                return response.Response;
            }

            if (response.Errors.All(e => e.Message.Contains("No sessions available") || e.Message.Contains("Session not found")))
            {
                throw new TelegramSessionNotFoundException();
            }

            throw new TelegramClientException(response.Errors);
        }

        public static IHttpClientBuilder AddTelegramClient(this IServiceCollection services, Uri telegramUrl)
        {
            return services.AddHttpClient<ITelegramClient, Client>((service, client) =>
            {
                client.BaseAddress = telegramUrl;
            });
        }
    }
}
