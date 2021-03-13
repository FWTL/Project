using System.Net.Http;
using System.Threading.Tasks;
using FWTL.TelegramClient.Responses;

namespace FWTL.TelegramClient.Services
{
    public class MessageService : BaseService, IMessageService
    {
        public MessageService(HttpClient client) : base(client)
        {
        }

        public Task<MessagesChats> GetAllChatsAsync(string sessionName)
        {
            return HandleAsync<MessagesChats>($"/api/users/{sessionName}/messages.getAllChats");
        }
    }
}