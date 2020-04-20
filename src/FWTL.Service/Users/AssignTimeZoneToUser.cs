using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace FWTL.Domain.Users
{
    public class AssignTimeZoneToUser
    {
        public class Request : IRequest
        {
            public string ZoneId { get; set; }
        }

        public class Command : Request, ICommand
        {
            public Command()
            {
                    
            }

            public Command(ICurrentUserService currentUserService)
            {
                CurrentUserId = currentUserService.CurrentUser;
            }

            public Guid CurrentUserId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly UserManager<User> _userManager;

            public IList<IEvent> Events => new List<IEvent>();

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public async Task ExecuteAsync(Command command)
            {
                var user = await _userManager.FindByIdAsync(command.CurrentUserId.ToString());
                user.TimeZoneId = command.ZoneId;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(ITimeZonesService timeZonesService)
            {
                RuleFor(x => x.ZoneId).NotEmpty()
                    .Must(timeZonesService.Exist).WithMessage("ZoneId doesn't exists");
            }
        }
    }
}