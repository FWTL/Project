using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Helpers;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.Domain.Traits;
using FWTL.TelegramClient;

namespace FWTL.Domain.Accounts
{
    public class VerifyAccount
    {
        public class Request : IRequest
        {
            public string AccountId { get; set; }

            public string Code { get; set; }
        }

        public class Command : Request, ICommand, ISessionNameTrait
        {
            public Command()
            {
            }

            public Command(ICurrentUserService currentUserService)
            {
                UserId = currentUserService.CurrentUserId;
            }

            public Guid UserId { get; set; }
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
                RuleFor(x => x.Code).Matches(RegexExpressions.OnlyNumbers);
            }
        }
    }
}