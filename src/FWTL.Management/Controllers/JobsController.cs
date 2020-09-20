using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FWTL.Management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICurrentUserService _currentUserService;

        public JobsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, ICurrentUserService currentUserService)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Authorize]
        public Task Start()
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Authorize]
        public Task Cancel(Guid jobId)
        {
            throw new NotImplementedException();
        }
    }
}