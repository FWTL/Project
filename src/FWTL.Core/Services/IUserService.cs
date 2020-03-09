using System;
using System.Collections.Generic;
using System.Text;

namespace FWTL.Core.Services
{
    public interface IUserService
    {
        object UserInfo(string region, string authorization);
    }
}
