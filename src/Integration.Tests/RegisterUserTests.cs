using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FWTL.Domain.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Integration.Tests
{
    [TestClass]
    public class RegisterUserTests
    {
        private static HttpClient _client;

        [ClassInitialize]
        public static async Task TestFixtureSetup(TestContext context)
        {
            _client = await new ApplicationFactory().CreateAsync();
        }

        [TestMethod]
        public async Task RegisterUser()
        {
            var content = JsonConvert.SerializeObject(new RegisterUser.RegisterUserRequest()
            {
                PhoneNumber = "4823142131",
                Password = "P@@444w0rd",
                RepeatPassword = "P@@444w0rd"
            });

            var httpResponse = await _client.PostAsync("/api/Users", new StringContent(content, Encoding.UTF8, "application/json"));
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}