using Newtonsoft.Json;
using System.Collections.Generic;
using FWTL.TelegramClient.Responses;

namespace FWTL.TelegramClient
{
    public class ResponseWrapper<TResponse>
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }

        public IEnumerable<Error> Errors { get; set; } = new List<Error>();

        public TResponse Response { get; set; }
    }
}