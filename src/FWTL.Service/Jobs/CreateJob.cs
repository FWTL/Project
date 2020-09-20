using FluentValidation;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.Domain.Traits;
using System;

namespace FWTL.Domain.Jobs
{
    public class CreateJob
    {
        public class Request : IRequest
        {
            public string AccountId { get; set; }

            public string DialogId { get; set; }
        }

        public class Command : Request, ISessionNameTrait, ICommand
        {
            public Command()
            {
            }

            public Command(ICurrentUserService currentUserService)
            {
            }

            public Guid UserId { get; set; }
        }

        public class Validator : AppAbstractValidation<Command>
        {
            public Validator()
            {
                RuleFor(x => x.DialogId).NotEmpty();
            }
        }
    }
}