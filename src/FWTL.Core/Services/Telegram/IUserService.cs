using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Services.Dto;
using FWTL.TelegramClient;

namespace FWTL.Core.Services.Telegram
{
    public interface IUserService
    {
        Task<ResponseWrapper> PhoneLoginAsync(string sessionName, string phoneNumber);

        Task<ResponseWrapper> CompletePhoneLoginAsync(string sessionName, string code);

        Task<ResponseWrapper<User>> GetSelfAsync(string sessionName);

        Task<ResponseWrapper> LogoutAsync(string sessionName);

        Task<ResponseWrapper<List<Dialog>>> GetDialogsAsync(string sessionName);
    }
}