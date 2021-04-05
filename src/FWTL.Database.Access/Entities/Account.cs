using System;
using Dapper.Contrib.Extensions;

namespace FWTL.Database.Access.Entities
{
    public class Account
    {
        [ExplicitKey]
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string ExternalAccountId { get; set; }
    }
}