using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Services.Telegram.Dto;

namespace FWTL.Core.Services.Telegram
{
    public interface IUserService
    {
        Task<ResponseWrapper> PhoneLoginAsync(Guid accountId, string phoneNumber);

        Task<ResponseWrapper> CompletePhoneLoginAsync(Guid accountId, string code);

        Task<ResponseWrapper<User>> GetSelfAsync(string sessionName);

        Task<ResponseWrapper<List<Dialog>>> GetDialogsAsync(string sessionName);
    }
}