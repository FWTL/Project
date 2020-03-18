using FWTL.TelegramClient.Responses;
using RestSharp;

namespace FWTL.TelegramClient.Services
{
    public class UserService : IUserService
    {
        private readonly IRestClient _client;

        public UserService(IRestClient client)
        {
            _client = client;
        }

        public void CompletePhoneLogin(string sessionName, string code)
        {
            var response = _client.Handle<GetSessionListResponse>($"api/users/{sessionName}aa/completePhoneLogin?code={code}");
        }

        public void PhoneLogin(string sessionName, string phoneNumber)
        {
            var response = _client.Handle<GetSessionListResponse>($"/api/users/{sessionName}/phoneLogin?phone={phoneNumber}");
        }
    }
}