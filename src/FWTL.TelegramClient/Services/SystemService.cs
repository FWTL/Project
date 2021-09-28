using System;
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

        public Task<ResponseWrapper> AddSessionAsync(Guid accountId)
        {
            return HandleAsync($"Accounts/{accountId}/system/addSession?session={accountId}");
        }

        public Task<ResponseWrapper> RemoveSessionAsync(Guid accountId)
        {
            return HandleAsync($"Accounts/{accountId}/system/removeSession?session={accountId}");
        }
    }
}