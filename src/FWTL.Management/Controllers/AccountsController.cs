using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.Domain.Accounts;
using FWTL.Domain.Accounts.AccountSetup;
using Microsoft.AspNetCore.Mvc;

namespace FWTL.Management.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
        public async Task<IReadOnlyList<GetAccounts.Result>> GetTelegramAccounts()
        {
            return await _queryDispatcher.DispatchAsync<GetAccounts.Query, IReadOnlyList<GetAccounts.Result>>(new GetAccounts.Query(_currentUserService));
        }

        [HttpPost]
        public async Task<Guid> AddTelegramAccount(string externalAccountId)
        {
            return await _commandDispatcher.DispatchAsync<AddAccount.Request, AddAccount.Command>(new AddAccount.Request() { ExternalAccountId = externalAccountId });
        }

        [HttpPost("{accountId}/Reset")]
        public async Task<Guid> ResetTelegramAccount(string externalAccountId)
        {
            return await _commandDispatcher.DispatchAsync<AddAccount.Request, AddAccount.Command>(new AddAccount.Request() { ExternalAccountId = externalAccountId });
        }

        [HttpPost("{accountId}/Verify")]
        public async Task VerifyTelegramAccount(Guid accountId, string code)
        {
            await _commandDispatcher.DispatchAsync<VerifyAccount.Request, VerifyAccount.Command>(new VerifyAccount.Request() { AccountId = accountId, Code = code });
        }

        [HttpDelete("{accountId}")]
        public async Task DeleteAccount(string accountId)
        {
            await _commandDispatcher.DispatchAsync<DeleteAccount.Request, DeleteAccount.Command>(new DeleteAccount.Request() { AccountId = accountId });
        }
    }
}