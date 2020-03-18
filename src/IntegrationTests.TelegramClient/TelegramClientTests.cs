using FWTL.TelegramClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Integration.TelegramClientTests
{
    [TestClass]
    public class TelegramClientTests
    {
        private static TelegramClient _client;

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            _client = new TelegramClient("http://127.0.0.1:9503");
        }

        [TestMethod]
        public void GetSessionList()
        {
            _client.SystemService.GetSessionList();
        }

        [TestMethod]
        public void AddSession()
        {
            _client.SystemService.AddSession("test");
        }
    }
}
