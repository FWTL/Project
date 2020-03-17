namespace FWTL.TelegramClient
{
    public interface ITelegramClient
    {
        IUserService UserService { get; }
        ISystemService SystemService { get; }
    }
}