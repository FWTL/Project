using FluentValidation;
using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using FWTL.Domain.Traits;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using FluentValidation.Results;
using FWTL.Common.Commands;
using FWTL.Common.Services;

namespace FWTL.RabbitMq
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _context;
        private readonly IGuidService _guidService;
        private readonly RequestToCommandMapper _requestToCommandMapper;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public CommandDispatcher(
            IServiceProvider context,
            ISendEndpointProvider sendEndpointProvider,
            IGuidService guidService,
            RequestToCommandMapper requestToCommandMapper,
            CommandFactory commandFactory)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
            _guidService = guidService;
            _requestToCommandMapper = requestToCommandMapper;
        }

        public async Task<Guid> DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand
        {
            IValidator<TCommand> validator = _context.GetService<IValidator<TCommand>>();
            if (validator.IsNotNull())
            {
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }

            Guid correlationId = _guidService.New;

            ISendEndpoint endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:commands"));
            await endpoint.Send(command, x => { x.CorrelationId = correlationId; });
            return correlationId;
        }

        private async Task TraitValidationAsync<TCommand, TTraitValidator>(TCommand command) where TCommand : class, ICommand
        {
            if (command is TTraitValidator)
            {
                IValidator<TTraitValidator> pagingValidator = _context.GetService<IValidator<TTraitValidator>>();
                ValidationResult pagingValidatorResult = await pagingValidator.ValidateAsync(command);
                if (!pagingValidatorResult.IsValid)
                {
                    throw new ValidationException(pagingValidatorResult.Errors);
                }
            }
        }

        public async Task<Guid> DispatchAsync<TRequest, TCommand>(TRequest request)
            where TCommand : class, ICommand
            where TRequest : class, IRequest
        {
            TCommand command = _requestToCommandMapper.Map<TRequest, TCommand>(request);
            return await DispatchAsync(command);
        }

        public async Task<Guid> DispatchAsync<TRequest, TCommand>(TRequest request, Action<TCommand> afterMap)
            where TCommand : class, ICommand
            where TRequest : class, IRequest
        {
            TCommand command = _requestToCommandMapper.Map(request, afterMap);
            return await DispatchAsync(command);
        }
    }
}