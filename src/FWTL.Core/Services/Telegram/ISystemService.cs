using System.Threading.Tasks;
using FWTL.Core.Services.Dto;

namespace FWTL.Core.Services.Telegram
{
    public interface ISystemService
    {
        Task<GetSessionListResponse> GetSessionListAsync();

        Task AddSessionAsync(string sessionName);

        Task RemoveSessionAsync(string sessionName);

        Task UnlinkSessionFileAsync(string sessionName);
    }
}