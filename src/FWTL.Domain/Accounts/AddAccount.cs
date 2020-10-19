using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using System;
using System.Threading.Tasks;

namespace FWTL.Domain.Accounts
{
    public class AddAccount
    {
        public class Request : IRequest
        {
            public string ExternalAccountId { get; set; }
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
            private readonly IGuidService _guidService;
            private readonly IAggregateStore _aggregateStore;

            public Handler(IGuidService guidService, IAggregateStore aggregateStore)
            {
                _guidService = guidService;
                _aggregateStore = aggregateStore;
            }

            public async Task<IAggregateRoot> ExecuteAsync(Command command)
            {
                 var user = _aggregateStore.GetNew<UserAggregate>();
                //UserAggregate user = await _aggregateStore.GetByIdAsync<UserAggregate>(command.UserId);
                user.AddAccount(_guidService.New, command);

                return user;
                //await _telegramClient.SystemService.AddSessionAsync(command.SessionName());
                //await _telegramClient.UserService.PhoneLoginAsync(command.SessionName(), command.AccountId);
            }
        }
    }
}