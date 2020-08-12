using FWTL.TelegramClient.Exceptions;
using FWTL.TelegramClient.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public static async Task<TResponse> HandleAsync<TResponse>(this IRestClient client, string url, Func<Error, bool> handleErrors)
        {
            var request = new RestRequest(url);
            var response = await client.PostAsync<ResponseWrapper<TResponse>>(request);

            if (!response.IsSuccess && !response.Errors.All(handleErrors))
            {
                throw new TelegramClientException(response.Errors);
            }

            return response.Response;
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