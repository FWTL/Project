using System;
using System.Collections.Generic;
using System.Text;

namespace FWTL.Core.Commands
{
    public class CommandComposite
    {
        public CommandComposite(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; set; }

        public CommandMetadata Metadata { get; set; } = new CommandMetadata();
    }
}
