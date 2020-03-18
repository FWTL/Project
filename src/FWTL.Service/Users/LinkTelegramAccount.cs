using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Extensions;
using FWTL.Common.Helpers;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.TelegramClient;

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
            public Command(ICurrentUserService currentUserService)
            {
                CurrentUserId = currentUserService.CurrentUser;
            }

            public long? NormalizedPhoneNumber { get; private set; }

            public void NormalizePhoneNumber()
            {
                NormalizedPhoneNumber = RegexExpressions.ONLY_NUMBERS.Replace(PhoneNumber)?.To<long>();
            }

            public long CurrentUserId { get; set; }
        }

        public class Handler : ICommandHandlerAsync<Command>
        {
            private readonly ITelegramClient _telegramClient;

            public Handler(ITelegramClient telegramClient)
            {
                _telegramClient = telegramClient;
            }

            public IList<IEvent> Events { get; } = new List<IEvent>();

            public Task ExecuteAsync(Command command)
            {
                var addSessionResponse = _telegramClient.SystemService.AddSession(command.NormalizedPhoneNumber.ToString());
                if (addSessionResponse.IsSuccess)
                {
                    throw new ValidationException(addSessionResponse.GetErrors());
                }

                var phoneLoginRespone = _telegramClient.UserService.PhoneLogin(command.NormalizedPhoneNumber.ToString(), command.NormalizedPhoneNumber.ToString());
                //if (phoneLoginRespone.IsSuccess)
                //{
                //    throw new ValidationException(phoneLoginRespone.GetErrors());
                //}

                return Task.CompletedTask;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.NormalizedPhoneNumber).NotNull();
            }
        }
    }
}