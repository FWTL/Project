using FWTL.TelegramClient.Responses;
using System.Threading.Tasks;

namespace FWTL.TelegramClient
{
    public interface IContactService
    {
        Task<ContactsContacts> GetAllContactsAsync(string sessionName);

        Task<Info> GetInfoAsync(string sessionName, Dialog.DialogType type, int id);

        Task<Info> GetInfoAsync(string sessionName, string dialogId);
    }
}