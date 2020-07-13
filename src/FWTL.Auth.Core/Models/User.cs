using Microsoft.AspNetCore.Identity;
using System;

namespace FWTL.Core.Models
{
    public class User : IdentityUser<Guid>
    {
        public string TimeZoneId { get; set; }
    }
}