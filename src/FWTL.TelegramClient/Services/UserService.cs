using FWTL.TelegramClient.Exceptions;
using FWTL.TelegramClient.Responses;
using System.Collections.Generic;
using System.Linq;
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
            return HandleAsync($"api/acc/{sessionName}/completePhoneLogin?code={code}");
        }

        public async Task<User> GetSelfAsync(string sessionName)
        {
            try
            {
                return await HandleAsync<User>($"api/acc/{sessionName}/getSelf");
            }
            catch (TelegramSessionNotFoundException)
            {
                return null;
            }
            catch (JsonException) //This occurs when user has session, but it is not authenticated yet. Response returns success but in response body equals to false instead of an object O.o'
            {
                return null;
            }
        }

        public Task PhoneLoginAsync(string sessionName, string phoneNumber)
        {
            return HandleAsync($"/api/acc/{sessionName}/phoneLogin?phone={phoneNumber}");
        }

        public async Task LogoutAsync(string sessionName)
        {
            try
            {
                await HandleAsync($"/api/acc/{sessionName}/logout");
            }
            catch (TelegramSessionNotFoundException) { }
        }

        public async Task<List<Dialog>> GetDialogsAsync(string sessionName)
        {
            //it seems Telegram returns dialogs from least frequently used.
            var result = await HandleAsync<List<Dialog>>($"api/acc/{sessionName}/getDialogs");
            result.Reverse();
            return result;
        }
    }
}