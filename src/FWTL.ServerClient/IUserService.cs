using System.Collections.Generic;
using FWTL.TelegramClient.Responses;
using System.Threading.Tasks;

namespace FWTL.TelegramClient
{
    public interface IUserService
    {
        Task PhoneLoginAsync(string sessionName, string phoneNumber);

        Task CompletePhoneLoginAsync(string sessionName, string code);

        Task<User> GetSelfAsync(string sessionName);

        Task LogoutAsync(string sessionName);

        Task<List<Dialog>> GetDialogsAsync(string sessionName);
    }
}