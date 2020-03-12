using System.Threading.Tasks;

namespace FWTL.TelegramServerClient
{
    public interface IUserService
    {
        Task PhoneLogin(string phoneNumber);

        Task CompletePhoneLogin(string code);
    }
}
