using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Services.Dto;

namespace FWTL.Core.Services.Telegram
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