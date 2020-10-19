using FluentValidation;
using FWTL.Common.Commands;
using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using FWTL.Common.Services;
using FWTL.Domain.Traits;

namespace FWTL.RabbitMq
{
    public class RequestDispatcher : ICommandDispatcher
    {
        private readonly IClientFactory _clientFactory;
        private readonly IServiceProvider _context;
        private readonly RequestToCommandMapper _requestToCommandMapper;

        public RequestDispatcher(
            IServiceProvider context,
            IClientFactory clientFactory,
            RequestToCommandMapper requestToCommandMapper)
        {
            _context = context;
            _clientFactory = clientFactory;
            _requestToCommandMapper = requestToCommandMapper;
        }

        public async Task<Guid> DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand
        {
            await TraitValidationAsync<TCommand, ISessionNameTrait>(command);
            
            IValidator<TCommand> validator = _context.GetService<IValidator<TCommand>>();
            if (validator.IsNotNull())
            {
                ValidationResult validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }

            IRequestClient<TCommand> client = _clientFactory.CreateRequestClient<TCommand>(new Uri("queue:commands"), TimeSpan.FromMinutes(10));
            MassTransit.Response<Response> response = await client.GetResponse<Response>(command);

            if (response.Message.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new AppValidationException(response.Message.Errors);
            }

            if (response.Message.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new InvalidOperationException(response.Message.Id.ToString());
            }

            return response.Message.Id;
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
            var command = _requestToCommandMapper.Map<TRequest, TCommand>(request);
            return await DispatchAsync(command);
        }

        public async Task<Guid> DispatchAsync<TRequest, TCommand>(TRequest request, Action<TCommand> afterMap)
            where TCommand : class, ICommand
            where TRequest : class, IRequest
        {
            var command = _requestToCommandMapper.Map(request, afterMap);
            return await DispatchAsync(command);
        }
    }
}