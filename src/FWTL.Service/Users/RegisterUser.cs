using System.Threading.Tasks;
using FluentValidation;
using FWTL.Core.Commands;
using FWTL.Core.Validation;

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

        public class Handler : ICommandHandlerAsync<RegisterUserCommand>
        {
            public Handler()
            {
            }

            public Task ExecuteAsync(RegisterUserCommand command)
            {
                return Task.CompletedTask;
            }
        }

        public class Validator : AppAbstractValidation<RegisterUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.PhoneNumber).NotEmpty();
                RuleFor(x => x.Password).NotNull();
                RuleFor(x => x.RepeatPassword).NotNull();
                RuleFor(x => x.Password).Matches(x => x.RepeatPassword);
            }
        }
    }
}