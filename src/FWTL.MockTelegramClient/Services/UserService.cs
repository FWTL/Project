using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Services.Dto;
using FWTL.Core.Services.Telegram;

namespace FWTL.MockTelegramClient.Services
{
    public class UserService : IUserService
    {
        public Task CompletePhoneLoginAsync(string sessionName, string code)
        {
            return Task.CompletedTask;
        }

        public Task<List<Dialog>> GetDialogsAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> GetSelfAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }

        public Task LogoutAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }

        public Task PhoneLoginAsync(string sessionName, string phoneNumber)
        {
            return Task.CompletedTask;
        }
    }
}