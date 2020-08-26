using FWTL.TelegramClient.Exceptions;
using FWTL.TelegramClient.Responses;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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
            catch (TelegramSessionNotFoundException)
            {
                return null;
            }
            catch (JsonException) //This occurs when user has session, but it is not authenticated yet. Response returns success but in response returns false instead of object O.o'
            {
                return null;
            }
        }

        public Task PhoneLoginAsync(string sessionName, string phoneNumber)
        {
            return HandleAsync<AuthSentCode>($"/api/users/{sessionName}/phoneLogin?phone={phoneNumber}");
        }

        public async Task LogoutAsync(string sessionName)
        {
            try
            {
                await HandleAsync($"/api/users/{sessionName}/logout");
            }
            catch (TelegramSessionNotFoundException)
            {
            }
        }
    }
}