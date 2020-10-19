using FWTL.Core.Aggregates;
using System.Threading.Tasks;

namespace FWTL.Core.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task<IAggregateRoot> ExecuteAsync(TCommand command);
    }
}