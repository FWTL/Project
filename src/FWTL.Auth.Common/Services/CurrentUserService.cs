using FWTL.Core.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
            get { return Guid.Parse(_principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value); }
        }

        public string SessionName(string phoneNumber) => CurrentUserId + "/" + phoneNumber;
    }
}