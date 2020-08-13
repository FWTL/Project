using FWTL.TelegramClient.Responses;
using System.Threading.Tasks;

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