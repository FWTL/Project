using FWTL.TelegramClient.Responses;

namespace FWTL.TelegramClient
{
    public interface ISystemService
    {
        ResponseWrapper<GetSessionListResponse> GetSessionList();

        ResponseWrapper<GetSessionListResponse> AddSession(string sessionName);
    }
}