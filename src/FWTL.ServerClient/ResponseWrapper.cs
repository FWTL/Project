using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace FWTL.TelegramClient
{
    public class ResponseWrapper<TResponse>
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }

        public IEnumerable<string> Errors { get; set; } = new List<string>();

        public TResponse Response { get; set; }
    }
}
