using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWTL.Management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICurrentUserService _currentUserService;

        public AccountsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher,
            ICurrentUserService currentUserService)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IReadOnlyList<GetAccounts.Result>> GetTelegramAccounts()
        {
            return await _queryDispatcher.DispatchAsync<GetAccounts.Query, IReadOnlyList<GetAccounts.Result>>(
                new GetAccounts.Query(_currentUserService));
        }

        [HttpPost("{accountId}")]
        [Authorize]
        public async Task AddTelegramAccount(string accountId)
        {
            await _commandDispatcher.DispatchAsync<AddAccount.Request, AddAccount.Command>(
                new AddAccount.Request() { AccountId = accountId });
        }

        [HttpPost("{accountId}/Code")]
        [Authorize]
        public async Task SendCode(string accountId)
        {
            await _commandDispatcher.DispatchAsync<SendCode.Request, SendCode.Command>(new SendCode.Request()
            { AccountId = accountId });
        }

        [HttpPost("{accountId}/Verify")]
        [Authorize]
        public async Task VerifyTelegramAccount(string accountId, string code)
        {
            await _commandDispatcher.DispatchAsync<VerifyAccount.Request, VerifyAccount.Command>(
                new VerifyAccount.Request() { AccountId = accountId, Code = code });
        }

        [HttpDelete("{accountId}")]
        [Authorize]
        public async Task DeleteAccount(string accountId)
        {
            await _commandDispatcher.DispatchAsync<DeleteAccount.Request, DeleteAccount.Command>(
                new DeleteAccount.Request() { AccountId = accountId });
        }

        [HttpGet("{accountId}/Dialogs")]
        [Authorize]
        public async Task<IReadOnlyList<GetDialogs.Result>> GetDialogs(string accountId, int start, int limit,
            bool isForced)
        {
            return await _queryDispatcher
                .DispatchAsync<GetDialogs.Request, GetDialogs.Query, IReadOnlyList<GetDialogs.Result>>(
                    new GetDialogs.Request()
                    {
                        AccountId = accountId,
                        Start = start,
                        Limit = limit,
                        IsForced = isForced
                    });
        }

        [HttpGet("{accountId}/Jobs")]
        public Task GetJobs(string accountId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{accountId}/Dialogs/{dialogId}/Jobs")]
        public Task Create(string accountId, string dialogId)
        {
            throw new NotImplementedException();
        }
    }
}