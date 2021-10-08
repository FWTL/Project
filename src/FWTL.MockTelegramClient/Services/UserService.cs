using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Services.Telegram;
using FWTL.Core.Services.Telegram.Dto;

namespace FWTL.MockTelegramClient.Services
{
    public class UserService : IUserService
    {
        public Task<ResponseWrapper> PhoneLoginAsync(string sessionName, string phoneNumber)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseWrapper> CompletePhoneLoginAsync(string sessionName, string code)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseWrapper> PhoneLoginAsync(Guid accountId, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper> CompletePhoneLoginAsync(Guid accountId, string code)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<User>> GetSelfAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseWrapper> LogoutAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseWrapper<List<Dialog>>> GetDialogsAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }
    }
}