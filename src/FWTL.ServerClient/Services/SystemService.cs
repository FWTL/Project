using FWTL.TelegramClient.Exceptions;
using FWTL.TelegramClient.Responses;
using System.Net.Http;
using System.Threading.Tasks;

namespace FWTL.TelegramClient.Services
{
    public class SystemService : BaseService, ISystemService
    {
        public SystemService(HttpClient client) : base(client)
        {
        }

        public Task<GetSessionListResponse> GetSessionListAsync()
        {
            return HandleAsync<GetSessionListResponse>("/system/getSessionList");
        }

        public Task AddSessionAsync(string sessionName)
        {
            return HandleAsync($"/system/addSession?session=users/{sessionName}");
        }

        public async Task RemoveSessionAsync(string sessionName)
        {
            try
            {
                await HandleAsync($"/system/removeSession?session=users/{sessionName}");
            }
            catch (TelegramSessionNotFoundException)
            {

            }
        }

        public async Task UnlinkSessionFileAsync(string sessionName)
        {
            try
            {
                await HandleAsync($"/system/unlinkSessionFile?session=users/{sessionName}");
            }
            catch (TelegramSessionNotFoundException)
            {
               
            }
        }
    }
}