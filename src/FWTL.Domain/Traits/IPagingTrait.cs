using System;
using System.Collections.Generic;
using System.Text;

namespace FWTL.Domain.Traits
{
    public interface IPagingTrait
    {
        int Start { get; set; }
        int Limit { get; set; }
    }
}
