using System;
using FWTL.TelegramClient.Responses;
using RestSharp;

namespace FWTL.TelegramClient
{
    public class SystemService : ISystemService
    {
        private readonly IRestClient _client;

        public SystemService(IRestClient client)
        {
            _client = client;
        }

        public ResponseWrapper<GetSessionListResponse> GetSessionList()
        {
            return _client.Handle<GetSessionListResponse>("/system/getSessionList");
        }

        public ResponseWrapper<GetSessionListResponse> AddSession(string sessionName)
        {
            return _client.Handle<GetSessionListResponse>($"/system/addSession?session=users/{sessionName}");
        }
    }

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