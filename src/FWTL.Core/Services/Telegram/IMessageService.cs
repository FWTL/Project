using System.Threading.Tasks;
using FWTL.Core.Services.Telegram.Dto;

namespace FWTL.Core.Services.Telegram
{
    public interface IMessageService
    {
        Task<ResponseWrapper<MessagesChats>> GetAllChatsAsync(string sessionName);
    }
}