using FWTL.TelegramClient.Responses;
using System;
using System.Collections.Generic;
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

        public Task GetInfoAsync(string sessionName, Dialog.DialogType type, int id)
        {
            string dialogType = Enum.GetName(typeof(Dialog.DialogType), type).ToLower();
            var queryParams = new Dictionary<string, string>()
            {
                {"id", $"{dialogType}#{id}"}
            };

            return HandleAsync($"/api/users/{sessionName}/getInfo", queryParams);
        }
    }
}