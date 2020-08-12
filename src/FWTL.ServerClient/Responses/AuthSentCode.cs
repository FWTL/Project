namespace FWTL.TelegramClient.Responses
{
    public class AuthSentCode
    {
        public IAuthSentCodeType Type { get; set; }
        public string PhoneCodeHash { get; set; }
        public IAuthCodeType AuthCodeType { get; set; }
        public int? Timeout { get; set; }
    }
}