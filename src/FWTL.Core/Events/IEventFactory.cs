using System.Collections.Generic;
using FWTL.Core.Commands;

namespace FWTL.Core.Events
{
    public interface IEventFactory
    {
        IEnumerable<EventComposite> Make(IEnumerable<EventComposite> @event);
    }
}