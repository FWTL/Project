using System.Threading.Tasks;

namespace FWTL.Core.Commands
{
    public interface ICommandHandlerAsync<in TCommand> where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command);
    }
}