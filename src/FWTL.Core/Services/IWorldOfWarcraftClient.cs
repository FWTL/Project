using System;
using System.Collections.Generic;
using System.Text;

namespace FWTL.Core.Services
{
    public interface IWorldOfWarcraftClient
    {
        IUserService UserService { get; }
    }
}
