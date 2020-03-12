﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using FWTL.Common.Commands;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using MassTransit;

namespace FWTL.RabbitMq
{
    public class RequestDispatcher : ICommandDispatcher
    {
        private readonly IClientFactory _clientFactory;
        private readonly IRequestToCommandMapper _requestToCommandMapper;
        private readonly IComponentContext _context;

        public RequestDispatcher(
            IComponentContext context,
            IClientFactory clientFactory,
            IRequestToCommandMapper requestToCommandMapper)
        {
            _context = context;
            _clientFactory = clientFactory;
            _requestToCommandMapper = requestToCommandMapper;
        }

        public async Task<Guid> DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand
        {
            if (_context.TryResolve(out AppAbstractValidation<TCommand> validator))
            {
                var validationResult = validator.Validate(command);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }

            var client = _clientFactory.CreateRequestClient<TCommand>(new Uri("queue:commands"), timeout: TimeSpan.FromMinutes(10));
            var response = await client.GetResponse<Response>(command);
            if (response.Message.Errors.Any())
            {
                throw new AppValidationException(response.Message.Errors);
            }
            return response.Message.Id;
        }

        public async Task<Guid> DispatchAsync<TRequest, TCommand>(TRequest request) where TCommand : class, ICommand
        {
            var command = _requestToCommandMapper.Map<TRequest, TCommand>(request);
            return await DispatchAsync(command);
        }

        public async Task<Guid> DispatchAsync<TRequest, TCommand>(TRequest request, Action<TCommand> afterMap) where TCommand : class, ICommand
        {
            var command = _requestToCommandMapper.Map(request, afterMap);
            return await DispatchAsync(command);
        }
    }
}