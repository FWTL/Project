using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FWTL.Aggregate
{
    public class User : IdentityUser<Guid>
    {
        public virtual IEnumerable<TelegramAccount> TelegramAccounts { get; set; } = new List<TelegramAccount>();
    }
}