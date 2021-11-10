using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Commands;
using FWTL.Core.Events;

namespace FWTL.Core.Aggregates
{
    public interface IAggregateRoot
    {
        IServiceProvider Context { get; set; }

        IEnumerable<EventComposite> Events { get; }

        Guid Id { get; }

        long Version { get; set; }

        Task CommitAsync(IAggregateStore aggregateStore);

        public bool IsDeleted { get; set; }

        void AddCompletionEvent<TCommand>(TCommand command) where TCommand : ICommand;

        void AddValidationFailureEvent<TCommand>(TCommand command) where TCommand : ICommand;
    }
}