using System;
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

        public Guid CurrentUserId => Guid.Parse("fe511212-15ae-404a-b3df-bb7d300c1283");
    }
}