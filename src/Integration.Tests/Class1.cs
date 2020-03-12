using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FWTL.Auth.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
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
        public Task RegisterUser()
        {
            var x = 1;
            x.Should().Be(1);
            return Task.CompletedTask;
        }
    }
}