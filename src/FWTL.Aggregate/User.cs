using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FWTL.Aggregate
{
    public class User : IdentityUser<Guid>
    {
        public virtual ICollection<Account> TelegramAccounts { get; set; } = new List<Account>();
    }
}