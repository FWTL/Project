using System.Collections.Generic;

namespace FWTL.TelegramClient.Responses
{
    public class ContactsContacts
    {
        public IEnumerable<User> Users { get; set; } = new List<User>();
    }
}