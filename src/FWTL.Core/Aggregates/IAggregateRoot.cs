using FWTL.Core.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWTL.Core.Aggregates
{
    public interface IAggregateRoot
    {
        IServiceProvider Context { get; set; }

        IEnumerable<EventComposite> Events { get; }

        Guid Id { get; set; }

        long Version { get; set; }

        Task CommitAsync(IAggregateStore aggregateStore);
    }
}