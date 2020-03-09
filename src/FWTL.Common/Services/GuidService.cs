using System;
using FWTL.Core.Services;

namespace FWTL.Common.Services
{
    public class GuidService : IGuidService
    {
        public Guid New => Guid.NewGuid();
    }
}