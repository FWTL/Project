using System;
using System.Threading.Tasks;
using FWTL.Core.Services.Telegram.Dto;

namespace FWTL.Core.Services.Telegram
{
    public interface ISystemService
    {
        Task<ResponseWrapper> AddSessionAsync(Guid accountId);

        Task<ResponseWrapper> RemoveSessionAsync(Guid accountId);
    }
}