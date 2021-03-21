using System;
using System.Threading.Tasks;
using FWTL.Core.Services.Dto;
using FWTL.Core.Services.Telegram;
using FWTL.TelegramClient;

namespace FWTL.MockTelegramClient.Services
{
    public class ContactService : IContactService
    {
        public Task<ResponseWrapper<ContactsContacts>> GetAllContactsAsync(string sessionName)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<Info>> GetInfoAsync(string sessionName, Dialog.DialogType type, int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWrapper<Info>> GetInfoAsync(string sessionName, string dialogId)
        {
            throw new NotImplementedException();
        }
    }
}