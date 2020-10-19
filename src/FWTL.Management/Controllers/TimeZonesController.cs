using FWTL.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        public IDictionary<string,string> Get()
        {
            return _timeZonesService.GetAll();
        }
    }
}