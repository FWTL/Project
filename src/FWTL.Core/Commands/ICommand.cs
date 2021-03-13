using System;

namespace FWTL.Core.Commands
{
    public interface ICommand
    {
        Guid CorrelationId { get; set; }
    }
}