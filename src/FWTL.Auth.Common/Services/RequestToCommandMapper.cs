﻿using System;
using AutoMapper;
using FWTL.Core.Commands;
using FWTL.Core.Services;

namespace FWTL.Common.Services
{
    public class RequestToCommandMapper : IRequestToCommandMapper
    {
        private readonly IMapper _mapper;

        public RequestToCommandMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TCommand Map<TRequest, TCommand>(TRequest request) where TCommand : class, ICommand
        {
            return _mapper.Map<TRequest, TCommand>(request);
        }

        public TCommand Map<TRequest, TCommand>(TRequest request, Action<TCommand> afterMap) where TCommand : class, ICommand
        {
            var command = _mapper.Map<TRequest, TCommand>(request);
            afterMap(command);
            return command;
        }
    }
}