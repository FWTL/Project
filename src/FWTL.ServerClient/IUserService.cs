using System.Threading.Tasks;

namespace FWTL.TelegramClient
{
    public interface IUserService
    {
        void PhoneLogin(string sessionName, string phoneNumber);

        void CompletePhoneLogin(string sessionName, string code);
    }
}