using FluentValidation;
using FluentValidation.Results;
using FWTL.Common.Extensions;
using FWTL.Core.Aggregates;
using FWTL.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Specification;

namespace FWTL.Common.Aggregates
{
    public abstract class AggregateRoot<TAggregate> : IAggregateRoot
        where TAggregate : AggregateRoot<TAggregate>
    {
        private readonly List<EventComposite> _events = new List<EventComposite>();

        [JsonIgnore]
        public IServiceProvider Context { get; set; }

        [JsonIgnore]
        public IEnumerable<EventComposite> Events => _events;

        public Guid Id { get; set; }

        public long Version { get; set; } = -1;

        protected void AddEvent<TEvent>(TEvent @event) where TEvent : IEvent
        {
            (this as IApply<TEvent>)?.Apply(@event);

            var specificationFor = Context.GetService<ISpecificationFor<TAggregate, TEvent>>();

            if (specificationFor.IsNotNull())
            {
                var validator = specificationFor.Apply(@event);
                ValidationResult result = validator.Validate(this);
                if (!result.IsValid)
                {
                    throw new ValidationException(result.Errors);
                }
            }

            _events.Add(new EventComposite(@event));
        }

        public virtual async Task CommitAsync(IAggregateStore aggregateStore)
        {
            await aggregateStore.SaveAsync(this as TAggregate);
        }
    }
}