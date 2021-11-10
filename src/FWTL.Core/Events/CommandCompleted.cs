using System;
using FWTL.Core.Commands;

namespace FWTL.Core.Events
{
    public class CommandCompleted<TCommand> : IEvent where TCommand : ICommand
    {
        public CommandCompleted()
        { }

        public CommandCompleted(TCommand command)
        {
            CorrelationId = command.CorrelationId;
        }

        public Guid CorrelationId { get; set; }
    }
}