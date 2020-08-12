using System.Collections.Generic;
using FWTL.TelegramClient.Exceptions;
using RestSharp;
using System.Linq;
using System.Threading.Tasks;
using FWTL.TelegramClient.Responses;

namespace FWTL.TelegramClient.Services
{
    public static class Helpers
    {
        public static async Task<TResponse> HandleAsync<TResponse>(this IRestClient client, string url)
        {
            var request = new RestRequest(url);
            var response = await client.PostAsync<ResponseWrapper<TResponse>>(request);

            if (!response.IsSuccess)
            {
                throw new TelegramClientException(response.Errors);
            }

            return response.Response;
        }

        public static async Task<IEnumerable<Error>> HandleWithoutExceptions(this IRestClient client, string url)
        {
            var request = new RestRequest(url);
            var response = await client.PostAsync<ResponseWrapper>(request);
            return response.Errors;
        }

        public static async Task HandleAsyncWithoutSession(this IRestClient client, string url)
        {
            var request = new RestRequest(url);
            var response = await client.PostAsync<ResponseWrapper>(request);

            if (!response.IsSuccess && !response.Errors.All(e => e.Message.Contains("Session not found") || e.Message.Contains("No sessions available")))
            {
                throw new TelegramClientException(response.Errors);
            }
        }

        public static async Task HandleAsync(this IRestClient client, string url)
        {
            var request = new RestRequest(url);
            var response = await client.PostAsync<ResponseWrapper>(request);

            if (!response.IsSuccess)
            {
                throw new TelegramClientException(response.Errors);
            }
        }

        public static string ToPascalCase(this string source)
        {
            var list = source.ToList();
            list[0] = char.ToUpper(list[0]);
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i] == '.')
                {
                    list[i + 1] = char.ToUpper(list[i + 1]);
                    list.RemoveAt(i);
                    i++;
                }
            }

            return string.Concat(list);
        }
    }
}