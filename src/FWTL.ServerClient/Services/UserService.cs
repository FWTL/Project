using FWTL.TelegramClient.Responses;
using System.Net.Http;
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

        public Task<User> GetSelfAsync(string sessionName)
        {
            return HandleAsync<User>($"api/users/{sessionName}/getSelf");
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