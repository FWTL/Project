using System;
using System.Linq;
using System.Security.Claims;
using FWTL.Core.Services;
using Microsoft.AspNetCore.Http;

namespace FWTL.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext.User;
            CurrentUserId = Guid.Parse(user.Claims.First(c => c.Type == "sub").Value);
        }

        public Guid CurrentUserId { get; }
    }
}