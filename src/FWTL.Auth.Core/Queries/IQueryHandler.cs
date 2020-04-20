using System;
using System.Collections.Generic;
using System.Text;

namespace FWTL.Core.Queries
{
    public interface IQueryHandler<in TQuery,out TResult> where TQuery : IQuery
    {
        TResult Handle(TQuery query);
    }
}
