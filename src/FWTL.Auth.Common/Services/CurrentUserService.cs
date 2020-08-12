using System;
using System.Linq;
using System.Security.Claims;
using FWTL.Core.Services;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace FWTL.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal _principal;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _principal = httpContextAccessor.HttpContext.User;
        }

        public Guid CurrentUserId
        {
            get
            {
                return Guid.Parse(_principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
        }

        public ulong PhoneNumber
        {
            get
            {
                return 48536276554;
                //return ulong.Parse(_principal.Claims.First(c => c.Type == ClaimTypes.MobilePhone).Value);
            }
        }
    }
}