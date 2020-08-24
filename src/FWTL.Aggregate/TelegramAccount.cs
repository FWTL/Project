using System;

namespace FWTL.Aggregate
{
    public class TelegramAccount
    {
        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public string Id { get; set; }
    }
}