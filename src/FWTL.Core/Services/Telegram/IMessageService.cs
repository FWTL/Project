using System.Threading.Tasks;
using FWTL.TelegramClient.Responses;

namespace FWTL.TelegramClient
{
    public interface IMessageService
    {
        Task<MessagesChats> GetAllChatsAsync(string sessionName);
    }
}