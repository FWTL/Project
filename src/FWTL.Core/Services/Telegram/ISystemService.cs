using System.Threading.Tasks;
using FWTL.TelegramClient.Responses;

namespace FWTL.TelegramClient
{
    public interface ISystemService
    {
        Task<GetSessionListResponse> GetSessionListAsync();

        Task AddSessionAsync(string sessionName);

        Task RemoveSessionAsync(string sessionName);

        Task UnlinkSessionFileAsync(string sessionName);
    }
}