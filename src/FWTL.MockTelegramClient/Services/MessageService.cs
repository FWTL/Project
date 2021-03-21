using System.Threading.Tasks;
using FWTL.Core.Services.Dto;
using FWTL.Core.Services.Telegram;
using FWTL.TelegramClient;

namespace FWTL.MockTelegramClient.Services
{
    public class MessageService :  IMessageService
    {
        public Task<ResponseWrapper<MessagesChats>> GetAllChatsAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }
    }
}