using System.Collections.Generic;
using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FWTL.Management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, ICurrentUserService currentUserService)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task Register(RegisterUser.Request request)
        {
            await _commandDispatcher.DispatchAsync<RegisterUser.Request, RegisterUser.Command>(request);
        }

        [HttpGet("Me")]
        [Authorize]
        public async Task<GetMe.Result> Me()
        {
            return await _queryDispatcher.DispatchAsync<GetMe.Query, GetMe.Result>(new GetMe.Query(_currentUserService));
        }

        [HttpGet("Me/Accounts")]
        [Authorize]
        public async Task<IReadOnlyList<GetAccounts.Result>> GetTelegramAccounts()
        {
            return await _queryDispatcher.DispatchAsync<GetAccounts.Query, IReadOnlyList<GetAccounts.Result>>(new GetAccounts.Query(_currentUserService));
        }

        [HttpPost("Me/Accounts/{phoneNumber}")]
        [Authorize]
        public async Task AddTelegramAccount(string phoneNumber)
        {
            await _commandDispatcher.DispatchAsync<AddTelegramAccount.Request, AddTelegramAccount.Command>(new AddTelegramAccount.Request() { PhoneNumber = phoneNumber });
        }

        [HttpPost("Me/Accounts/{phoneNumber}/Verify")]
        [Authorize]
        public async Task VerifyTelegramAccount(string phoneNumber, string code)
        {
            await _commandDispatcher.DispatchAsync<VerifyAccount.Request, VerifyAccount.Command>(new VerifyAccount.Request() { PhoneNumber = phoneNumber, Code = code });
        }

        [HttpDelete("Me/Accounts/{phoneNumber}")]
        [Authorize]
        public async Task DeleteAccount(string phoneNumber)
        {
            await _commandDispatcher.DispatchAsync<DeleteAccount.Request, DeleteAccount.Command>(new DeleteAccount.Request() { PhoneNumber = phoneNumber });
        }
    }
}