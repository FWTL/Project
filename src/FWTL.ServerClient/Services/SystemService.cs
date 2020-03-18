using FWTL.TelegramClient.Responses;
using RestSharp;

namespace FWTL.TelegramClient.Services
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
}