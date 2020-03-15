using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace FWTL.Auth.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public UsersController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task RegisterUser(RegisterUser.RegisterUserRequest request)
        {
            await _commandDispatcher.DispatchAsync<RegisterUser.RegisterUserRequest, RegisterUser.RegisterUserCommand>(request, command => command.NormalizePhoneNumber());
        }
    }
}