using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.Client;
using FWTL.Common.Exceptions;
using FWTL.Core.Aggregates;
using FWTL.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using StreamPosition = EventStore.Client.StreamPosition;

namespace FWTL.EventStore
{
    public class EventStoreAggregateStore : IAggregateStore
    {
        private readonly IDatabase _cache;

        private readonly IServiceProvider _context;

        private readonly EventStoreClient _eventStoreClient;

        public EventStoreAggregateStore(
            EventStoreClient eventStoreClient,
            IDatabase cache,
            IServiceProvider context)
        {
            _eventStoreClient = eventStoreClient;
            _cache = cache;
            _context = context;
        }

        public async Task<TAggregate> GetByIdAsync<TAggregate>(Guid aggregateId) where TAggregate : class, IAggregateRoot, new()
        {
            var aggregate = await GetByIdOrDefaultAsync<TAggregate>(aggregateId.ToString(), int.MaxValue);
            if (aggregate is null)
            {
                throw new AppValidationException($"{typeof(TAggregate).Name}Id", $"Aggregate with id : {aggregate.Id} not found");
            }

            return aggregate;
        }

        public TAggregate GetNew<TAggregate>() where TAggregate : class, IAggregateRoot, new()
        {
            var model = new TAggregate { Context = _context };
            return model;
        }

        public async Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : class, IAggregateRoot
        {
            var streamName = $"{aggregate.GetType().Name}:{aggregate.Id}";
            var newEvents = aggregate.Events;
            var eventsToSave = newEvents.Select(ToEventData).ToList();

            var service = _context.GetService<IAggregateMap<TAggregate>>();
            if (service != null)
            {
                await service.SaveAsync(aggregate);
            }

            await _eventStoreClient.AppendToStreamAsync(streamName, StreamState.Any, eventsToSave);

            aggregate.Version += aggregate.Events.Count();
            await _cache.StringSetAsync(streamName, JsonConvert.SerializeObject(aggregate), TimeSpan.FromDays(1));
        }

        private dynamic DeserializeEvent(ReadOnlyMemory<byte> metadata, ReadOnlyMemory<byte> data)
        {
            var eventType = JObject.Parse(Encoding.UTF8.GetString(metadata.Span)).Property("EventType").Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data.Span), Type.GetType((string)eventType));
        }

        public async Task<bool> ExistsAsync<TAggregate>(string aggregateId) where TAggregate : class, IAggregateRoot
        {
            var streamName = $"{typeof(TAggregate).Name}:{aggregateId}";
            EventStoreClient.ReadStreamResult stream = _eventStoreClient.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.FromInt64(0));

            return await stream.ReadState != ReadState.StreamNotFound;
        }

        private async Task<TAggregate> GetByIdOrDefaultAsync<TAggregate>(string aggregateId, int version)
            where TAggregate : class, IAggregateRoot, new()
        {
            if (version <= 0)
            {
                throw new InvalidOperationException("Cannot get version <= 0");
            }

            var streamName = $"{typeof(TAggregate).Name}:{aggregateId}";
            TAggregate aggregate = new TAggregate();

            var value = await _cache.StringGetAsync(streamName);
            if (value.HasValue)
            {
                aggregate = JsonConvert.DeserializeObject<TAggregate>(value);
            }

            long sliceStart = aggregate.Version + 1;

            EventStoreClient.ReadStreamResult stream = _eventStoreClient.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.FromInt64(sliceStart));

            if (await stream.ReadState == ReadState.StreamNotFound)
            {
                return null;
            }

            await foreach (var @event in stream)
            {
                (aggregate as dynamic).Apply(DeserializeEvent(@event.Event.Metadata, @event.Event.Data));
                aggregate.Version++;
            }

            aggregate.Context = _context;
            return aggregate;
        }

        private EventData ToEventData(EventComposite @event)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event.Event));
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event.Metadata));

            return new EventData(@event.Metadata.EventId, @event.Metadata.EventType, data, metadata);
        }
    }
}