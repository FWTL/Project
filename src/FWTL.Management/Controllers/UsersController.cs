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
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(IQueryDispatcher queryDispatcher, ICurrentUserService currentUserService)
        {
            _queryDispatcher = queryDispatcher;
            _currentUserService = currentUserService;
        }

        [HttpGet("Me")]
        
        public async Task<GetMe.Result> Me()
        {
            return await _queryDispatcher.DispatchAsync<GetMe.Query, GetMe.Result>(new GetMe.Query(_currentUserService));
        }
    }
}