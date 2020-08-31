using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        [HttpPost("Me/Accounts/{accountId}")]
        [Authorize]
        public async Task AddTelegramAccount(string accountId)
        {
            await _commandDispatcher.DispatchAsync<AddTelegramAccount.Request, AddTelegramAccount.Command>(new AddTelegramAccount.Request() { AccountId = accountId });
        }

        [HttpPost("Me/Accounts/{accountId}/Code")]
        [Authorize]
        public async Task SendCode(string accountId)
        {
            await _commandDispatcher.DispatchAsync<SendCode.Request, SendCode.Command>(new SendCode.Request() { AccountId = accountId });
        }

        [HttpPost("Me/Accounts/{accountId}/Verify")]
        [Authorize]
        public async Task VerifyTelegramAccount(string accountId, string code)
        {
            await _commandDispatcher.DispatchAsync<VerifyAccount.Request, VerifyAccount.Command>(new VerifyAccount.Request() { AccountId = accountId, Code = code });
        }

        [HttpDelete("Me/Accounts/{accountId}")]
        [Authorize]
        public async Task DeleteAccount(string accountId)
        {
            await _commandDispatcher.DispatchAsync<DeleteAccount.Request, DeleteAccount.Command>(new DeleteAccount.Request() { AccountId = accountId });
        }

        [HttpGet("Me/Accounts/{accountId}/Dialogs")]
        [Authorize]
        public async Task<IReadOnlyList<GetDialogs.Result>> GetDialogs(string accountId)
        {
            return await _queryDispatcher.DispatchAsync<GetDialogs.Request, GetDialogs.Query, IReadOnlyList<GetDialogs.Result>>( new GetDialogs.Request() {AccountId = accountId});
        }
    }
}