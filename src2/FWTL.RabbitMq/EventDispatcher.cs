using System.Threading.Tasks;
using FWTL.Core.Events;

namespace FWTL.RabbitMq
{
    public class EventDispatcher : IEventDispatcher
    {
        public Task DispatchAsync(EventComposite @event)
        {
            return Task.CompletedTask;
        }
    }
}