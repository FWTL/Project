using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Domain.Users;
using FWTL.TelegramClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FWTL.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ITelegramClient _telegramClient;

        public AccountsController(ICommandDispatcher commandDispatcher, ITelegramClient telegramClient)
        {
            _commandDispatcher = commandDispatcher;
            _telegramClient = telegramClient;
        }

        //[HttpPost("{phoneNumber}/Code")]
        //public void PhoneLogin(string phoneNumber)
        //{
        //    _telegramClient.SystemService.AddSession(phoneNumber);
        //    _telegramClient.UserService.PhoneLogin(phoneNumber, phoneNumber);
        //}

        //[Authorize]
        //[HttpPatch("Timezone")]
        //public async Task AssignTimeZoneToUser(string zoneId)
        //{
        //    await _commandDispatcher.DispatchAsync<AssignTimeZoneToUser.Request, AssignTimeZoneToUser.Command>(new AssignTimeZoneToUser.Request() { ZoneId = zoneId });
        //}
    }
}