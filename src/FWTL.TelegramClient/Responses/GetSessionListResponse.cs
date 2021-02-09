using System.Collections.Generic;

namespace FWTL.TelegramClient.Responses
{
    public class GetSessionListResponse
    {
        public Dictionary<string, SessionInfo> Sessions { get; set; } = new Dictionary<string, SessionInfo>();

        public class SessionInfo
        {
            public string Session { get; set; }

            public string File { get; set; }

            public string Status { get; set; }
        }
    }
}