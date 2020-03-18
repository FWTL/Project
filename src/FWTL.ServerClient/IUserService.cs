using System.Threading.Tasks;

namespace FWTL.TelegramClient
{
    public interface IUserService
    {
        bool PhoneLogin(string sessionName, string phoneNumber);

        bool CompletePhoneLogin(string sessionName, string code);
    }
}