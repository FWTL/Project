using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Domain.Users;
using FWTL.TelegramClient;
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

        [HttpPatch("/CompletePhoneLogin")]
        public void CompletePhoneLogin(string phoneNumber, string code)
        {
            _telegramClient.UserService.CompletePhoneLogin(phoneNumber, code);
        }

        [HttpPatch("/Timezone")]
        public async Task SetTimeZone(string zoneId)
        {
            await _commandDispatcher.DispatchAsync(new AssignTimeZoneToUser.Command() { ZoneId = zoneId } );
        }
    }
}