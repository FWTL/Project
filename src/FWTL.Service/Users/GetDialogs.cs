using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.TelegramClient;
using FWTL.TelegramClient.Responses;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Domain.Cache;

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
            public Result()
            {
            }

            public Result(Dialog dialog, User user)
            {
                Id = dialog.Id;
                Type = (PeerType)dialog.Type;
            }

            public Result(Dialog dialog, Chat info)
            {
                Id = dialog.Id;
                Type = (PeerType)dialog.Type;
                Created = info.Date;
            }

            public int Id { get; set; }

            public string Name { get; set; }

            public PeerType Type { get; set; }

            public Instant? Created { get; set; }

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
            private readonly ICacheService _cache;

            public Handler(ITelegramClient telegramClient, ICacheService cache)
            {
                _telegramClient = telegramClient;
                _cache = cache;
            }

            public async Task<IReadOnlyList<Result>> HandleAsync(Query query)
            {
                var dialogs = await _cache.GetAsync(
                    CacheHelper.GetDialogsAsync(query),
                    () => _telegramClient.UserService.GetDialogsAsync(query.SessionName),
                    TimeSpan.FromHours(1));

                var results = new List<Result>();
                foreach (var dialog in dialogs.Skip(query.Start).Take(query.Limit))
                {
                    var info = await _cache.GetAsync(
                        CacheHelper.GetInfoAsync(query,dialog),
                        () => _telegramClient.ContactService.GetInfoAsync(query.SessionName, dialog.Type, dialog.Id),
                        TimeSpan.FromDays(5));

                    results.Add(info.Chat.IsNotNull() ? new Result(dialog, info.Chat) : new Result(dialog, info.User));
                }

                return results;
            }
        }
    }
}