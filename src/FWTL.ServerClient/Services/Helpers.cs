using System;
using RestSharp;

namespace FWTL.TelegramClient.Services
{
    public static class Helpers
    {
        public static ResponseWrapper<TResponse> Handle<TResponse>(this IRestClient client, string url)
        {
            var request = new RestRequest(url);
            var response = client.Post<ResponseWrapper<TResponse>>(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return response.Data;
        }
    }
}