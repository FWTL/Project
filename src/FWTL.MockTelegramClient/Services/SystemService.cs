using System.Threading.Tasks;
using FWTL.Core.Services.Dto;
using FWTL.Core.Services.Telegram;

namespace FWTL.MockTelegramClient.Services
{
    public class SystemService : ISystemService
    {
        public Task<GetSessionListResponse> GetSessionListAsync()
        {
            return Task.FromResult(new GetSessionListResponse());
        }

        public Task AddSessionAsync(string sessionName)
        {
            return Task.CompletedTask;
        }

        public Task RemoveSessionAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }

        public Task UnlinkSessionFileAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }
    }
}