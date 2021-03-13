using FWTL.Core.Queries;
using FWTL.Core.Services;
using Microsoft.AspNetCore.Mvc;

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
    }
}