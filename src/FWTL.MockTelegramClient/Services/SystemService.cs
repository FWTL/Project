using System.Threading.Tasks;
using FWTL.Core.Services.Telegram;
using FWTL.Core.Services.Telegram.Dto;

namespace FWTL.MockTelegramClient.Services
{
    public class SystemService : ISystemService
    {
        public Task<ResponseWrapper<GetSessionListResponse>> GetSessionListAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseWrapper> AddSessionAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseWrapper> RemoveSessionAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseWrapper> UnlinkSessionFileAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }
    }
}