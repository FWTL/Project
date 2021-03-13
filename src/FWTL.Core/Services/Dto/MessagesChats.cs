using System.Collections.Generic;

namespace FWTL.TelegramClient.Responses
{
    public class MessagesChats
    {
        public IEnumerable<Chat> Chats { get; set; } = new List<Chat>();
    }
}