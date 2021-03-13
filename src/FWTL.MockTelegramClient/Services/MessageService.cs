using System.Threading.Tasks;
using FWTL.Core.Services.Dto;
using FWTL.Core.Services.Telegram;

namespace FWTL.MockTelegramClient.Services
{
    public class MessageService :  IMessageService
    {
        public Task<MessagesChats> GetAllChatsAsync(string sessionName)
        {
            throw new System.NotImplementedException();
        }
    }
}