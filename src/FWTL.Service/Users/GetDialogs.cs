using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.TelegramClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Aggregate;
using static System.String;

namespace FWTL.Domain.Users
{
    public class GetDialogs
    {
        public class Request : IRequest
        {
            public string AccountId { get; set; }

            public int Limit { get; set; }

            public int Start { get; set; }
        }

        public class Query : Request, IQuery
        {
            public Query()
            {
            }

            public Query(ICurrentUserService currentUserService)
            {
                UserId = currentUserService.CurrentUserId;
            }

            public string SessionName => UserId.ToSession(AccountId);

            public Guid UserId { get; set; }
        }

        public class Result
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public PeerType Type { get; set; }

            public int? MigratedFromChat { get; set; }

            public enum PeerType
            {
                User = 1,
                Chat = 2,
                Channel = 3,
            }
        }

        public class Handler : IQueryHandler<Query, IReadOnlyList<Result>>
        {
            private readonly ITelegramClient _telegramClient;

            public Handler(ITelegramClient telegramClient)
            {
                _telegramClient = telegramClient;
            }

            public async Task<IReadOnlyList<Result>> HandleAsync(Query query)
            {
                var dialogs = await _telegramClient.UserService.GetDialogsAsync(query.SessionName);
                foreach (var dialog in dialogs)
                {
                    await _telegramClient.ContactService.GetInfoAsync(query.SessionName, dialog.Type, dialog.Id);
                }
                return null;
            }
        }
    }
}