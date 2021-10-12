using System;
using System.Threading.Tasks;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;

namespace FWTL.Domain.Session
{
    public class RemoveSession
    {
        public class Command : ICommand
        {
            public Guid CorrelationId { get; set; }

            public Guid AccountId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            public Handler()
            {
            }

            public Task<IAggregateRoot> ExecuteAsync(Command command)
            {
                return Task.FromResult<IAggregateRoot>(null);
            }
        }
    }
}