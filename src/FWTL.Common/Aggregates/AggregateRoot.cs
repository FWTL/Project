using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FWTL.Common.Extensions;
using FWTL.Core.Aggregates;
using FWTL.Core.Events;
using FWTL.Core.Specification;
using Microsoft.Extensions.DependencyInjection;

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

        public long Version { get; set; }

        public Guid Id { get; set; }

        public bool ToDelete { get; set; }

        public bool IsDeleted { get; set; }

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
            if (ToDelete)
            {
                await aggregateStore.DeleteAsync(this as TAggregate);
                return;
            }
            await aggregateStore.SaveAsync(this as TAggregate);
        }

        public virtual void SoftDelete()
        {
            IsDeleted = true;
        }

        public virtual void Delete()
        {
            ToDelete = true;
        }
    }
}