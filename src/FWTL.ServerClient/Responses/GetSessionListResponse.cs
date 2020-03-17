using System.Collections.Generic;

namespace FWTL.TelegramClient.Responses
{
    public class GetSessionListResponse
    {
        public Dictionary<string, SessionInfo> Sessions = new Dictionary<string, SessionInfo>();

        public class SessionInfo
        {
            public string Session { get; set; }

            public string File { get; set; }

            public string Status { get; set; }
        }
    }

    public static class SessionInfoStatus
    {
        public const string NOT_LOGGED_IN = "NOT_LOGGED_IN";
        public const string WAITING_CODE = "WAITING_CODE";
        public const string WAITING_PASSWORD = "WAITING_PASSWORD";
        public const string WAITING_SIGNUP = "WAITING_SIGNUP";
        public const string LOGGED_IN = "LOGGED_IN";
    }
}