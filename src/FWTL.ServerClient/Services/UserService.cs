using FWTL.TelegramClient.Responses;
using RestSharp;
using System.Threading.Tasks;

namespace FWTL.TelegramClient.Services
{
    public class UserService : IUserService
    {
        private readonly IRestClient _client;

        public UserService(IRestClient client)
        {
            _client = client;
        }

        public async Task CompletePhoneLoginAsync(string sessionName, string code)
        {
            await _client.HandleAsync<AuthAuthorization>($"api/users/{sessionName}/completePhoneLogin?code={code}");
        }

        public Task<User> GetSelfAsync(string sessionName)
        {
            return _client.HandleAsync<User>($"api/users/{sessionName}/getSelf");
        }

        public async Task PhoneLoginAsync(string sessionName, string phoneNumber)
        {
            await _client.HandleAsync<AuthSentCode>($"/api/users/{sessionName}/phoneLogin?phone={phoneNumber}");
        }
    }
}