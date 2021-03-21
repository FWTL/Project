using System.Collections.Generic;

namespace FWTL.Core.Services.Telegram.Dto
{
    public class ContactsContacts
    {
        public IEnumerable<User> Users { get; set; } = new List<User>();
    }
}