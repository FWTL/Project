using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FWTL.Auth.Database.Entities;
using FWTL.Common.Extensions;
using FWTL.Common.Helpers;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Validation;
using Microsoft.AspNetCore.Identity;

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
            public long? NormalizedPhoneNumber { get; private set; }

            public void NormalizePhoneNumber()
            {
                NormalizedPhoneNumber = RegexExpressions.ONLY_NUMBERS.Replace(PhoneNumber)?.To<long>();
            }
        }

        public class Handler : ICommandHandlerAsync<RegisterUserCommand>
        {
            private readonly UserManager<User> _userManager;
            private RoleManager<Role> _roleManager;

            public Handler(UserManager<User> userManager, RoleManager<Role> roleManager)
            {
                _userManager = userManager;
                _roleManager = roleManager;
            }

            public IList<IEvent> Events => new List<IEvent>();

            public async Task ExecuteAsync(RegisterUserCommand command)
            {
                var user = new User()
                {
                    Id = command.NormalizedPhoneNumber.Value,
                    UserName = command.NormalizedPhoneNumber.Value.ToString()
                };

                var createUserResult = await _userManager.CreateAsync(user, command.Password);
                if (!createUserResult.Succeeded)
                {
                    var errors = createUserResult.Errors.Select(x => new ValidationFailure(nameof(RegisterUserRequest), x.Description));
                    throw new AppValidationException(errors);
                }

                var addUserToRoleResult = await _userManager.AddToRoleAsync(user, "user");
                if (!addUserToRoleResult.Succeeded)
                {
                    var errors = addUserToRoleResult.Errors.Select(x => new ValidationFailure(nameof(RegisterUserRequest), x.Description));
                    throw new AppValidationException(errors);
                }
            }
        }

        public class Validator : AppAbstractValidation<RegisterUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.PhoneNumber).NotEmpty();
                RuleFor(x => x).Must(x => x.NormalizedPhoneNumber.HasValue).WithMessage("Incorrect Phone Number");
                RuleFor(x => x.Password).NotNull();
                RuleFor(x => x.RepeatPassword).NotNull();
                RuleFor(x => x.Password).Equal(x => x.RepeatPassword);
            }
        }
    }
}