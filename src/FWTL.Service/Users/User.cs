using Microsoft.AspNetCore.Identity;

namespace FWTL.Domain.Users
{
    public class User : IdentityUser<long>
    {
        public string TimeZone { get; set; }
    }
}