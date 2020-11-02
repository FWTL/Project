using System;
using MassTransit;

namespace FWTL.Core.Commands
{
    public interface ICommand 
    {
        Guid CorrelationId { get; set; }
    }
}