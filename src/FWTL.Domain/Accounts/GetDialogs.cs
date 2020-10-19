using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.Domain.Cache;
using FWTL.Domain.Traits;
using FWTL.TelegramClient;
using FWTL.TelegramClient.Responses;
using NodaTime;

namespace FWTL.Domain.Accounts
{
    public class GetDialogs
    {
        public class Request : IRequest, IPagingTrait
        {
            public string AccountId { get; set; }

            public int Limit { get; set; }

            public int Start { get; set; }

            public bool IsForced { get; set; }
        }

        public class Query : Request, IQuery, ISessionNameTrait
        {
            public Query()
            {
            }

            public Query(ICurrentUserService currentUserService)
            {
                UserId = currentUserService.CurrentUserId;
            }

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
                Type = dialog.Type;

                if (user.IsDeleted)
                {
                    Name = "Deleted";
                }
                else
                {
                    List<string> nameBuilder = new List<string>();
                    if (!string.IsNullOrEmpty(user.Firstname))
                    {
                        nameBuilder.Add(user.Firstname);
                    }

                    if (!string.IsNullOrEmpty(user.Lastname))
                    {
                        nameBuilder.Add(user.Lastname);
                    }

                    if (!string.IsNullOrEmpty(user.Username))
                    {
                        nameBuilder.Add($"[{user.Username}]");
                    }

                    Name = string.Join(" ", nameBuilder);
                }
            }

            public Result(Dialog dialog, Chat info)
            {
                Id = dialog.Id;
                Type = dialog.Type;
                CreatedAt = info.Date;
                Name = info.Title;
            }

            public int Id { get; set; }

            public string Name { get; set; }

            public Dialog.DialogType Type { get; set; }

            public Instant? CreatedAt { get; set; }
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
                    () => _telegramClient.UserService.GetDialogsAsync(query.SessionName()),
                    query.IsForced,
                    TimeSpan.FromDays(5));

                var results = new List<Result>();
                foreach (var dialog in dialogs.Skip(query.Start).Take(query.Limit))
                {
                    var info = await _cache.GetAsync(
                        CacheHelper.GetInfoAsync(query, dialog),
                        () => _telegramClient.ContactService.GetInfoAsync(query.SessionName(), dialog.Type, dialog.Id),
                        query.IsForced,
                        TimeSpan.FromDays(5));

                    results.Add(info.Chat.IsNotNull() ? new Result(dialog, info.Chat) : new Result(dialog, info.User));
                }

                return results;
            }
        }

        public class Validator : AppAbstractValidation<Query>
        {
            public Validator(IValidator<ISessionNameTrait> validator)
            {
                RuleFor(x => x).SetValidator(validator);
            }
        }
    }
}