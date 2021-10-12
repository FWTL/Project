using System.Collections.Generic;
using FWTL.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FWTL.Management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TimeZonesController : ControllerBase
    {
        private readonly ITimeZonesService _timeZonesService;

        public TimeZonesController(ITimeZonesService timeZonesService)
        {
            _timeZonesService = timeZonesService;
        }

        [HttpGet]
        public IDictionary<string, string> Get()
        {
            return _timeZonesService.GetAll();
        }
    }
}