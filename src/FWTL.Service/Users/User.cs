using Microsoft.AspNetCore.Identity;

namespace FWTL.Auth.Database.Entities
{
    public class User : IdentityUser<long>
    {
        public string TimeZone { get; set; }
    }
}