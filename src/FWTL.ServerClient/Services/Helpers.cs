using RestSharp;
using System.Threading.Tasks;

namespace FWTL.TelegramClient.Services
{
    public static class Helpers
    {
        public static async Task<ResponseWrapper<TResponse>> HandleAsync<TResponse>(this IRestClient client, string url)
        {
            var request = new RestRequest(url);
            return await client.PostAsync<ResponseWrapper<TResponse>>(request);
        }
    }
}