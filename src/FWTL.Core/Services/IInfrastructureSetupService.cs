using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FWTL.Core.Services
{
    public interface IInfrastructureService
    {
        public Task<Result> CreateTelegramApi(Guid accountId);
        public Task<Result> DeleteTelegramApi(Guid accountId);
    }

    public class Result
    {
        public Result()
        {
            IsSuccess = true;
        }

        public Result(IEnumerable<string> errors)
        {
            IsSuccess = false;
            Errors = errors;
        }

        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
