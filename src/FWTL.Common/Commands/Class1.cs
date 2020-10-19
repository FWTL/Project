using FWTL.Core.Commands;
using FWTL.Core.Services;

namespace FWTL.Common.Commands
{
    public class CommandFactory
    {
        private readonly IGuidService _guid;

        public CommandFactory(IGuidService guid)
        {
            _guid = guid;
        }

        public CommandComposite Make(ICommand command)
        {
            var commandComposite = new CommandComposite(command);
            commandComposite.Metadata.CorrelationId = _guid.New;

            return commandComposite;
        }
    }
}