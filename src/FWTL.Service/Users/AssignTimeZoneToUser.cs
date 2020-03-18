using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FWTL.Core.Commands;
using FWTL.Core.Services;

namespace FWTL.Domain.Users
{
    public class AssignTimeZoneToUser
    {
        public class Command : ICommand
        {
            public string ZoneId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(ITimeZonesService timeZonesService)
            {
                RuleFor(x => x.ZoneId).NotEmpty()
                    .Must(timeZonesService.AnyExist).WithMessage("ZoneId doesn't exists");
            }
        }
    }
}
