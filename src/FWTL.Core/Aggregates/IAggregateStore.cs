﻿using System;
using System.Threading.Tasks;

namespace FWTL.Core.Aggregates
{
    public interface IAggregateStore
    {
        Task<TAggregate> GetByIdOrDefaultAsync<TAggregate>(TAggregate aggregate) where TAggregate : class, IAggregateRoot, new();

        Task<TAggregate> GetByIdAsync<TAggregate>(Guid aggregateId) where TAggregate : class, IAggregateRoot, new();

        Task<TAggregate> GetByIdAsync<TAggregate>(string aggregateId) where TAggregate : class, IAggregateRoot, new();

        Task<bool> ExistsAsync<TAggregate>(string aggregateId) where TAggregate : class, IAggregateRoot;

        TAggregate GetNew<TAggregate>() where TAggregate : class, IAggregateRoot, new();

        Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : class, IAggregateRoot;
    }
}