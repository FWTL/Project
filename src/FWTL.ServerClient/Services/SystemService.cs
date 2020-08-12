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

        public Task<GetSessionListResponse> GetSessionListAsync()
        {
            return _client.HandleAsync<GetSessionListResponse>("/system/getSessionList");
        }

        public Task AddSessionAsync(string sessionName)
        {
            return _client.HandleAsync($"/system/addSession?session=users/{sessionName}");
        }

        public async Task RemoveSession(string sessionName)
        {
            await _client.HandleAsyncWithoutSession($"/api/users/{sessionName}/logout");
            await _client.HandleAsyncWithoutSession($"/system/removeSession?session=users/{sessionName}");
            await _client.HandleWithoutExceptions($"/system/unlinkSessionFile?session=users/{sessionName}"); //seems not to work
        }
    }
}