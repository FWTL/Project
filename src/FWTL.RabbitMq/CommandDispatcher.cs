using System;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.RabbitMq
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _context;
        private readonly IGuidService _guidService;
        private readonly IRequestToCommandMapper _requestToCommandMapper;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public CommandDispatcher(
            IServiceProvider context,
            ISendEndpointProvider sendEndpointProvider,
            IGuidService guidService,
            IRequestToCommandMapper requestToCommandMapper)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
            _guidService = guidService;
            _requestToCommandMapper = requestToCommandMapper;
        }

        public async Task<Guid> DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand
        {
            AppAbstractValidation<TCommand> validator = _context.GetService<AppAbstractValidation<TCommand>>();
            if (validator.IsNotNull())
            {
                var validationResult = validator.Validate(command);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }

            var correlationId = _guidService.New;

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:commands"));
            await endpoint.Send(command, x => { x.CorrelationId = correlationId; });
            return correlationId;
        }

        public async Task<Guid> DispatchAsync<TRequest, TCommand>(TRequest request)
            where TCommand : class, ICommand
        {
            var command = _requestToCommandMapper.Map<TRequest, TCommand>(request);
            return await DispatchAsync(command);
        }

        public async Task<Guid> DispatchAsync<TRequest, TCommand>(TRequest request, Action<TCommand> afterMap)
            where TCommand : class, ICommand
        {
            var command = _requestToCommandMapper.Map(request, afterMap);
            return await DispatchAsync(command);
        }
    }
}