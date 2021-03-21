using System.Threading.Tasks;
using FWTL.Core.Services.Dto;
using FWTL.TelegramClient;

namespace FWTL.Core.Services.Telegram
{
    public interface ISystemService
    {
        Task<ResponseWrapper<GetSessionListResponse>> GetSessionListAsync();

        Task<ResponseWrapper> AddSessionAsync(string sessionName);

        Task<ResponseWrapper> RemoveSessionAsync(string sessionName);

        Task<ResponseWrapper> UnlinkSessionFileAsync(string sessionName);
    }
}