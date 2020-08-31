using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.TelegramClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.String;

namespace FWTL.Domain.Users
{
    public class GetDialogs
    {
        public class Request : IRequest
        {
            public string AccountId { get; set; }
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
                string sessionName = query.UserId.ToSession(query.AccountId);
                var allContacts = await _telegramClient.ContactService.GetAllContactsAsync(sessionName);
                var allChats = await _telegramClient.MessageService.GetAllChatsAsync(sessionName);

                var dialogs = new List<Result>();
                dialogs.AddRange(allContacts.Users.Select(u =>
                {
                    string name = u.Firstname;
                    if (!IsNullOrEmpty(u.Lastname))
                    {
                        name += " " + u.Lastname;
                    }
                    name = IsNullOrEmpty(name) ? u.Username : name;

                    return new Result()
                    {
                        Id = u.Id,
                        Type = Result.PeerType.User,
                        Name = name
                    };
                }));

                dialogs.AddRange(allChats.Chats.Where(u => u.MigratedTo.IsNull()).Select(chat => new Result()
                {
                    Id = chat.Id,
                    Type = chat.Type == "chat" ? Result.PeerType.Chat : Result.PeerType.Channel,
                    Name = chat.Title,
                    MigratedFromChat = allChats.Chats.Where(x => x.MigratedTo?.ChannelId == chat.Id).Select(x => (int?)x.Id).FirstOrDefault()
                }));

                dialogs = dialogs.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList();

                return dialogs;
            }
        }
    }
}