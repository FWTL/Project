using System;
using System.Threading.Tasks;
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

        public bool CompletePhoneLogin(string sessionName, string code)
        {
            //var response = _client.HandleAsync<GetSessionListResponse>($"api/users/{sessionName}aa/completePhoneLogin?code={code}");
            //return response.IsSuccess;
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<User>> GetSelfAsync(string sessionName)
        {
            return _client.HandleAsync<User>($"api/users/{sessionName}/getSelf");
        }

        public bool PhoneLogin(string sessionName, string phoneNumber)
        {
            var response = _client.HandleAsync<AuthSentCode>($"/api/users/{sessionName}/phoneLogin?phone={phoneNumber}");
            //return response.IsSuccess;
            throw new NotImplementedException();
        }
    }
}