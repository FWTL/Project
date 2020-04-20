using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FWTL.Auth.Controllers
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
        public async Task RegisterUser(RegisterUser.Request request)
        {
            await _commandDispatcher.DispatchAsync<RegisterUser.Request, RegisterUser.Command>(request);
        }

        //[HttpGet("Me")]
        //[Authorize]
        //public async Task Me()
        //{
        //    await _commandDispatcher.DispatchAsync<LinkTelegramAccount.Request, LinkTelegramAccount.Command>(new LinkTelegramAccount.Request() { PhoneNumber = phoneNumber }, command => command.NormalizePhoneNumber());
        //}

        [HttpPost("Me/Link/Telegram")]
        [Authorize]
        public async Task PhoneLogin(string phoneNumber)
        {
            await _commandDispatcher.DispatchAsync<LinkTelegramAccount.Request, LinkTelegramAccount.Command>(new LinkTelegramAccount.Request() { PhoneNumber = phoneNumber }, command => command.NormalizePhoneNumber());
        }

        [Authorize]
        [HttpPatch("Me/Timezone")]
        public async Task AssignTimeZoneToUser(string zoneId)
        {
            await _commandDispatcher.DispatchAsync<AssignTimeZoneToUser.Request, AssignTimeZoneToUser.Command>(new AssignTimeZoneToUser.Request() { ZoneId = zoneId });
        }
    }
}