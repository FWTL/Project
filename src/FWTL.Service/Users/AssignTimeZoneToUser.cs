using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using Microsoft.AspNetCore.Identity;

namespace FWTL.Domain.Users
{
    public class AssignTimeZoneToUser
    {
        public class Request : IRequest
        {
            public string TimeZoneId { get; set; }
        }

        public class Command : Request, ICommand
        {
            public Command()
            {
            }

            public Command(ICurrentUserService currentUserService)
            {
                UserId = currentUserService.CurrentUserId;
            }

            public Guid UserId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly UserManager<User> _userManager;

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public IList<IEvent> Events { get; } = new List<IEvent>();

            public async Task ExecuteAsync(Command command)
            {
                User user = await _userManager.FindByIdAsync(command.UserId.ToString());
                user.TimeZoneId = command.TimeZoneId;

                var assignTimeZoneToUserResult = await _userManager.UpdateAsync(user);
                if (!assignTimeZoneToUserResult.Succeeded)
                {
                    throw new ValidationException(assignTimeZoneToUserResult.GetErrors());
                }
            }

            public class Validator : AppAbstractValidation<Command>
            {
                public Validator(ITimeZonesService timeZonesService)
                {
                    RuleFor(x => x.TimeZoneId).Must(timeZonesService.Exist);
                }
            }
        }
    }
}