using System.Collections.Generic;

namespace FWTL.Core.Events
{
    public interface IEventFactory
    {
        IEnumerable<EventComposite> Make(IEnumerable<EventComposite> @events);
    }
}