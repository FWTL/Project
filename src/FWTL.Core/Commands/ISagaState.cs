using System;
using System.Collections.Generic;
using System.Text;

namespace FWTL.Core.Commands
{
    public interface ISagaState
    {
        public string ResponseAddress { get; set; }
        public Guid RequestId { get; set; }
    }
}
