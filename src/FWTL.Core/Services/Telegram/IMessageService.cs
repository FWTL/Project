using System.Threading.Tasks;
using FWTL.Core.Services.Dto;

namespace FWTL.Core.Services.Telegram
{
    public interface IMessageService
    {
        Task<MessagesChats> GetAllChatsAsync(string sessionName);
    }
}