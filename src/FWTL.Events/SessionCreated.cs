using System;
using FWTL.Core.Events;

namespace FWTL.Events
{
    public class SessionCreated : IEvent
    {
        public Guid AccountId { get; set; }
    }
}