using System.Threading.Tasks;
using FWTL.Core.Aggregates;

namespace FWTL.Core.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task<IAggregateRoot> ExecuteAsync(TCommand command);
    }
}