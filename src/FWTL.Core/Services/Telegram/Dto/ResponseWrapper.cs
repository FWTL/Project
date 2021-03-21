using System.Collections.Generic;
using System.Text.Json.Serialization;
using FWTL.Core.Services.Dto;

namespace FWTL.TelegramClient
{
    public class ResponseWrapper<TResponse> : ResponseWrapper
    {
        public TResponse Response { get; set; }
    }

    public class ResponseWrapper
    {
        [JsonPropertyName("success")]
        public bool IsSuccess { get; set; }

        public IEnumerable<Error> Errors { get; set; } = new List<Error>();
    }
}