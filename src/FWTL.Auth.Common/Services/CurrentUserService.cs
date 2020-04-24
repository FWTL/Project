using System;
using System.Linq;
using System.Security.Claims;
using FWTL.Core.Services;
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
                return Guid.Parse(_principal.Claims.First(c => c.Type == "sub").Value);
            }
        }
    }
}