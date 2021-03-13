using System.Threading.Tasks;
using FWTL.Core.Services.Dto;

namespace FWTL.Core.Services.Telegram
{
    public interface IContactService
    {
        Task<ContactsContacts> GetAllContactsAsync(string sessionName);

        Task<Info> GetInfoAsync(string sessionName, Dialog.DialogType type, int id);

        Task<Info> GetInfoAsync(string sessionName, string dialogId);
    }
}