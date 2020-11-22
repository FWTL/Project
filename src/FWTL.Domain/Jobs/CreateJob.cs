using System;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using FWTL.Core.Validation;

namespace FWTL.Domain.Jobs
{
    public class CreateJob
    {
        public class Request : IRequest
        {
            public Guid AccountId { get; set; }

            public string DialogId { get; set; }
        }

        public class Command : Request, ICommand
        {
            public Command()
            {
            }

            public Command(ICurrentUserService currentUserService, IGuidService guidService)
            {
                UserId = currentUserService.CurrentUserId;
                Id = guidService.New;
            }

            public Guid UserId { get; set; }

            public Guid Id { get; set; }
            public Guid CorrelationId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            public Task<IAggregateRoot> ExecuteAsync(Command command)
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