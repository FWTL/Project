using System;
using Microsoft.AspNetCore.Identity;

namespace FWTL.Domain.Users
{
    public class User : IdentityUser<Guid>
    {
        public string TimeZoneId { get; set; }
    }
}