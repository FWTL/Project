using System;
using System.Threading.Tasks;
using FWTL.Core.Commands;

namespace FWTL.Core.Aggregates
{
    public interface IAggregateStore
    {
        Task<TAggregate> GetByIdAsync<TAggregate>(Guid aggregateId, bool isDeleted = false) where TAggregate : class, IAggregateRoot, new();

        Task<TAggregate> GetByIdAsync<TAggregate, TCommand>(Guid aggregateId, TCommand command, bool isDeleted = false)
            where TAggregate : class, IAggregateRoot, new() where TCommand : ICommand;

        TAggregate GetNew<TAggregate>() where TAggregate : class, IAggregateRoot, new();

        Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : class, IAggregateRoot;

        Task DeleteAsync<TAggregate>(TAggregate aggregate) where TAggregate : class, IAggregateRoot;

        Task<TAggregate> GetByIdOrDefaultAsync<TAggregate>(Guid aggregateId) where TAggregate : class, IAggregateRoot, new();
    }
}