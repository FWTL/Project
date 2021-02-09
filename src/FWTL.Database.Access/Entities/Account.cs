using System;

namespace FWTL.Database.Access.Entities
{
    public class Account
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string ExternalAccountId { get; set; }
    }
}