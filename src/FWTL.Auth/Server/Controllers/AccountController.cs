using FWTL.TelegramServerClient;
using Microsoft.AspNetCore.Mvc;

namespace FWTL.Auth.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ITelegramServerClient _telegramServerClient;

        public AccountController(ITelegramServerClient telegramServerClient)
        {
            _telegramServerClient = telegramServerClient;
        }

        [HttpPost]
        public void SendCode(string phoneNumber)
        {
            //_telegramServerClient.
        }

        [HttpPost]
        public void Register()
        {
        }
    }
}