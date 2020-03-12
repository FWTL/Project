using System;
using FWTL.Core.Commands;

namespace FWTL.Core.Services
{
    public interface IRequestToCommandMapper
    {
        TCommand Map<TRequest, TCommand>(TRequest request) where TCommand : class, ICommand;

        TCommand Map<TRequest, TCommand>(TRequest request, Action<TCommand> afterMap) where TCommand : class, ICommand;
    }
}