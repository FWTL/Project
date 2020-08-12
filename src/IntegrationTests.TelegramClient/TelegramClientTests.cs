using System.Threading.Tasks;
using FWTL.TelegramClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Integration.TelegramClientTests
{
    [TestClass]
    public class TelegramClientTests
    {
        private static Client _client;

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            _client = new Client("http://127.0.0.1:9503");
        }

        [TestMethod]
        public async Task GetSessionList()
        {
            await _client.SystemService.GetSessionListAsync();
        }

        [TestMethod]
        public async Task AddSession()
        {
            await _client.SystemService.AddSessionAsync("test");
        }
    }
}
