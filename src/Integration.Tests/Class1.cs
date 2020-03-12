using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var response = await _client.GetAsync("api/Account");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}