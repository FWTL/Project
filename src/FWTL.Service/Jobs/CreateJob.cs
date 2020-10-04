using FluentValidation;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.Domain.Traits;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Events;

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

        public class Handler : ICommandHandler<Command>
        {
            public IList<IEvent> Events { get; }

            public Task ExecuteAsync(Command command)
            {
                throw new NotImplementedException();
            }
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