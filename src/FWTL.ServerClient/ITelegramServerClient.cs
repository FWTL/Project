namespace FWTL.TelegramServerClient
{
    public interface ITelegramServerClient
    {
        IUserService UserService { get; }
        ISystemService SystemService { get; }
    }
}