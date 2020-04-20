using System.Collections.Generic;
using System.Threading.Tasks;
using FWTL.Core.Events;

namespace FWTL.Core.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        IList<IEvent> Events { get; }

        Task ExecuteAsync(TCommand command);
    }
}