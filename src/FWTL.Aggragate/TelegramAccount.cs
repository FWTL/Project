using System;

namespace FWTL.Aggragate
{
    public class TelegramAccount
    {
        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public string Number { get; set; }
    }
}