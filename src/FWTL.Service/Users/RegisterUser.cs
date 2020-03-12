using FWTL.Core.Commands;

namespace FWTL.Domain.Users
{
    public class RegisterUser
    {
        public class RegisterUserRequest : IRequest
        {
            public string PhoneNumber { get; set; }

            public string Password { get; set; }

            public string RepeatPassword { get; set; }
        }

        public class RegisterUserCommand : RegisterUserRequest, ICommand
        {
        }
    }
}