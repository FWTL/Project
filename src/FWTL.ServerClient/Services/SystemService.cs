using FWTL.TelegramClient.Responses;
using RestSharp;
using System.Threading.Tasks;

namespace FWTL.TelegramClient.Services
{
    public class SystemService : ISystemService
    {
        private readonly IRestClient _client;

        public SystemService(IRestClient client)
        {
            _client = client;
        }

        public Task<ResponseWrapper<GetSessionListResponse>> GetSessionListAsync()
        {
            return _client.HandleAsync<GetSessionListResponse>("/system/getSessionList");
        }

        public Task<ResponseWrapper<GetSessionListResponse>> AddSessionAsync(string sessionName)
        {
            return _client.HandleAsync<GetSessionListResponse>($"/system/addSession?session=users/{sessionName}");
        }
    }
}