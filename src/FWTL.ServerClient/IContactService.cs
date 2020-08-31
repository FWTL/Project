using FWTL.TelegramClient.Responses;
using System.Threading.Tasks;

namespace FWTL.TelegramClient
{
    public interface IContactService
    {
        Task<ContactsContacts> GetAllContactsAsync(string sessionName);
    }
}