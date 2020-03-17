using System.Collections.Generic;
using FWTL.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FWTL.Auth.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeZonesController : ControllerBase
    {
        private readonly ITimeZonesService _timeZonesService;

        public TimeZonesController(ITimeZonesService timeZonesService)
        {
            _timeZonesService = timeZonesService;
        }

        [HttpGet]
        public IEnumerable<string> Get(string phoneNumber, string code)
        {
            return _timeZonesService.GetAll();
        }
    }
}