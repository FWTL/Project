using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Enums;
using FWTL.Core.Events;
using FWTL.Core.Validation;
using FWTL.Events;
using Microsoft.AspNetCore.Identity;

namespace FWTL.Domain.Users
{
    public class RegisterUser
    {
        public class Request : IRequest
        {
            public string Email { get; set; }

            public string Password { get; set; }

            public string RepeatPassword { get; set; }
        }

        public class Command : Request, ICommand
        {
        }

        public class Handler : ICommandHandlerAsync<Command>
        {
            private readonly UserManager<User> _userManager;
           
            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public IList<IEvent> Events => new List<IEvent>();

            public async Task ExecuteAsync(Command command)
            {
                var user = new User()
                {
                    UserName = command.Email,
                    Email = command.Email,
                    EmailConfirmed = false
                };

                var createUserResult = await _userManager.CreateAsync(user, command.Password);
                if (!createUserResult.Succeeded)
                {
                    throw new ValidationException(createUserResult.GetErrors());
                }

                var addUserToRoleResult = await _userManager.AddToRoleAsync(user, nameof(Roles.User));
                if (!addUserToRoleResult.Succeeded)
                {
                    throw new ValidationException(addUserToRoleResult.GetErrors());
                }

                string activationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                Events.Add(new UserRegistered()
                {
                    Email = user.Email,
                    ActivationCode = activationCode
                });
            }
        }

        public class Validator : AppAbstractValidation<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Email).EmailAddress();
                RuleFor(x => x.Password).NotNull();
                RuleFor(x => x.RepeatPassword).NotNull();
                RuleFor(x => x.Password).Equal(x => x.RepeatPassword);
            }
        }
    }
}