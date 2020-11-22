using System;

namespace FWTL.Database.Entities
{
    public class Account
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string ExternalAccountId { get; set; }
    }
}