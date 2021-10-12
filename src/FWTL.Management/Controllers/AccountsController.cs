using System;
using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Domain.Accounts.AccountSetup;
using FWTL.Domain.Accounts.DeleteAccount;
using FWTL.Domain.Accounts.RestartSetup;
using Microsoft.AspNetCore.Mvc;

namespace FWTL.Management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public AccountsController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<Guid> AddTelegramAccount(string externalAccountId)
        {
            return await _commandDispatcher.DispatchAsync<AddAccount.Request, AddAccount.Command>(new AddAccount.Request() { ExternalAccountId = externalAccountId });
        }

        [HttpPost("{accountId}/Reset")]
        public async Task<Guid> ResetTelegramAccount(Guid accountId)
        {
            return await _commandDispatcher.DispatchAsync<ResetSetup.Request, ResetSetup.Command>(new ResetSetup.Request() { AccountId = accountId });
        }

        [HttpPost("{accountId}/Verify")]
        public async Task VerifyTelegramAccount(Guid accountId, string code)
        {
            await _commandDispatcher.DispatchAsync<VerifyAccount.Request, VerifyAccount.Command>(new VerifyAccount.Request() { AccountId = accountId, Code = code });
        }

        [HttpDelete]
        public async Task DeleteTelegramAccount(Guid accountId)
        {
            await _commandDispatcher.DispatchAsync<DeleteAccount.Request, DeleteAccount.Command>(new DeleteAccount.Request() { AccountId = accountId });
        }
    }
}