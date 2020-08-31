using FWTL.TelegramClient.Responses;
using System.Threading.Tasks;

namespace FWTL.TelegramClient
{
    public interface IMessageService
    {
        Task<MessagesChats> GetAllChatsAsync(string sessionName);
    }
}