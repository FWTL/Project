using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FWTL.Core.Services.Telegram;
using FWTL.Core.Services.Telegram.Dto;

namespace FWTL.TelegramClient.Services
{
    public class UserService : BaseService, IUserService
    {
        public UserService(HttpClient client) : base(client)
        {
        }

        public Task<ResponseWrapper> CompletePhoneLoginAsync(Guid accountId, string code)
        {
            return HandleAsync($"Accounts/{accountId}/api/completePhoneLogin?code={code}");
        }

        public Task<ResponseWrapper<List<Dialog>>> GetDialogsAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseWrapper<User>> GetSelfAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseWrapper> PhoneLoginAsync(Guid accountId, string phoneNumber)
        {
            return HandleAsync($"Accounts/{accountId}/api/phoneLogin?phone={phoneNumber}");
        }
    }
}