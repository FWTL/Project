using System;
using System.Threading.Tasks;
using FWTL.Core.Services.Telegram;
using FWTL.Core.Services.Telegram.Dto;

namespace FWTL.MockTelegramClient.Services
{
    public class SystemService : ISystemService
    {
        public Task<ResponseWrapper> AddSessionAsync(Guid accountId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper> RemoveSessionAsync(Guid accountId)
        {
            throw new NotImplementedException();
        }
    }
}