using FluentValidation;
using FWTL.Common.Helpers;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.TelegramClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FWTL.Domain.Users
{
    public class LinkTelegramAccount
    {
        public class Request : IRequest
        {
            public string PhoneNumber { get; set; }
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
            private readonly ITelegramClient _telegramClient;

            public IList<IEvent> Events => new List<IEvent>();

            public Handler(ITelegramClient telegramClient)
            {
                _telegramClient = telegramClient;
            }

            public async Task ExecuteAsync(Command command)
            {
                var addSessionResponse = await _telegramClient.SystemService.AddSessionAsync(command.PhoneNumber);
                bool doesSessionAlreadyExist = !addSessionResponse.IsSuccess &&
                                               addSessionResponse.Errors.Any(x =>
                                                   x.Message == "Session already exists");
                if (doesSessionAlreadyExist)
                {
                  return;
                }

                _telegramClient.UserService.PhoneLogin(command.PhoneNumber, command.PhoneNumber);
            }
        }

        public class Validator : AppAbstractValidation<Command>
        {
            public Validator()
            {
                RuleFor(x => x.PhoneNumber).Matches(RegexExpressions.ONLY_NUMBERS);
            }
        }
    }
}