using FWTL.TelegramClient.Responses;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FWTL.TelegramClient
{
    public class ResponseWrapper<TResponse> : ResponseWrapper
    {
        public TResponse Response { get; set; }
    }

    public class ResponseWrapper
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }

        public IEnumerable<Error> Errors { get; set; } = new List<Error>();
    }
}