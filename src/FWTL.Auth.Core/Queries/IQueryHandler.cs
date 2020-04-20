using System;
using System.Collections.Generic;
using System.Text;

namespace FWTL.Core.Queries
{
    public interface IQueryHandler<in TQuery,> where TCommand : ICommand
    {

    }
}
