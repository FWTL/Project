using FluentValidation;
using FWTL.Common.Helpers;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.Domain.Mixins;
using FWTL.TelegramClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWTL.Domain.Users
{
    public class VerifyAccount
    {
        public class Request : IRequest
        {
            public string AccountId { get; set; }

            public string Code { get; set; }
        }

        public class Command : Request, ICommand, ISessionNameMixin
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
            private readonly ITelegramClient _telegramClient;

            public Handler(ITelegramClient telegramClient)
            {
                _telegramClient = telegramClient;
            }

            public IList<IEvent> Events => new List<IEvent>();

            public async Task ExecuteAsync(Command command)
            {
                await _telegramClient.UserService.CompletePhoneLoginAsync(command.SessionName(), command.Code);
            }
        }

        public class Validator : AppAbstractValidation<Command>
        {
            public Validator()
            {
                RuleFor(x => x.AccountId).Matches(RegexExpressions.ONLY_NUMBERS);
                RuleFor(x => x.Code).Matches(RegexExpressions.ONLY_NUMBERS);
            }
        }
    }
}