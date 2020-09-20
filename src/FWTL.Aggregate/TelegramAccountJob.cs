using System;

namespace FWTL.Aggregate
{
    public class TelegramAccountJob
    {
        public Guid JobId { get; set; }

        public Guid TelegramAccountId { get; set; }

        public virtual TelegramAccount TelegramAccount { get; set; }

        public virtual Job Job { get; set; }
    }
}