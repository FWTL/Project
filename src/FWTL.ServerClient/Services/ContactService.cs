using FWTL.TelegramClient.Responses;
using System.Net.Http;
using System.Threading.Tasks;

namespace FWTL.TelegramClient.Services
{
    public class ContactService : BaseService, IContactService
    {
        public ContactService(HttpClient client) : base(client)
        {
        }

        public Task<ContactsContacts> GetAllContactsAsync(string sessionName)
        {
            return HandleAsync<ContactsContacts>($"/api/users/{sessionName}/contacts.getContacts");
        }
    }
}