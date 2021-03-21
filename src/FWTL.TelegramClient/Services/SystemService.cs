using System.Net.Http;
using System.Threading.Tasks;
using FWTL.Core.Services.Telegram;
using FWTL.Core.Services.Telegram.Dto;

namespace FWTL.TelegramClient.Services
{
    public class SystemService : BaseService, ISystemService
    {
        public SystemService(HttpClient client) : base(client)
        {
        }

        public Task<ResponseWrapper> AddSessionAsync(string sessionName)
        {
            return HandleAsync($"/system/addSession?session=acc/{sessionName}");
        }

        public Task<ResponseWrapper<GetSessionListResponse>> GetSessionListAsync()
        {
            return HandleAsync<GetSessionListResponse>("/system/getSessionList");
        }

        public Task<ResponseWrapper> RemoveSessionAsync(string sessionName)
        {
            return HandleAsync($"/system/removeSession?session=acc/{sessionName}");
        }

        public Task<ResponseWrapper> UnlinkSessionFileAsync(string sessionName)
        {
            return HandleAsync($"/system/unlinkSessionFile?session=acc/{sessionName}");
        }
    }
}