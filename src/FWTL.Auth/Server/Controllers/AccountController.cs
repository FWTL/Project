using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Domain.Users;
using FWTL.TelegramServerClient;
using Microsoft.AspNetCore.Mvc;

namespace FWTL.Auth.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ICommandDispatcher _commandDispatcher;

        public AccountController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [HttpGet]
        public void Test()
        {
            //_telegramServerClient.
        }

        [HttpPost]
        public async Task Register(RegisterUser.RegisterUserRequest request)
        {
            await _commandDispatcher.DispatchAsync<RegisterUser.RegisterUserRequest, RegisterUser.RegisterUserCommand>(request);
        }
    }
}