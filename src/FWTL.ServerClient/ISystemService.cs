using System.Threading.Tasks;
using FWTL.TelegramClient.Responses;

namespace FWTL.TelegramClient
{
    public interface ISystemService
    {
        Task<ResponseWrapper<GetSessionListResponse>> GetSessionListAsync();

        Task<ResponseWrapper<GetSessionListResponse>> AddSessionAsync(string sessionName);
    }
}