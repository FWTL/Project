using FWTL.TelegramClient.Responses;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using FWTL.TelegramClient.Exceptions;

namespace FWTL.TelegramClient.Services
{
    public class UserService : BaseService, IUserService
    {
        public UserService(HttpClient client) : base(client)
        {
        }

        public Task CompletePhoneLoginAsync(string sessionName, string code)
        {
            return HandleAsync<AuthAuthorization>($"api/users/{sessionName}/completePhoneLogin?code={code}");
        }

        public async Task<User> GetSelfAsync(string sessionName)
        {
            try
            {
                return await HandleAsync<User>($"api/users/{sessionName}/getSelf");
            }
            catch (TelegramClientException e)
            {
                return null;
            }
        }

        public Task PhoneLoginAsync(string sessionName, string phoneNumber)
        {
            return HandleAsync<AuthSentCode>($"/api/users/{sessionName}/phoneLogin?phone={phoneNumber}");
        }

        public Task LogoutAsync(string sessionName)
        {
            return HandleAsync($"/api/users/{sessionName}/logout");
        }
    }
}