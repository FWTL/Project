using System.Collections.Generic;

namespace FWTL.Core.Services.Telegram.Dto
{
    public class MessagesChats
    {
        public IEnumerable<Chat> Chats { get; set; } = new List<Chat>();
    }
}