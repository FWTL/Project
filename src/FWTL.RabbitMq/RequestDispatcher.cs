using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FWTL.Common.Cqrs.Mappers;
using FWTL.Common.Exceptions;
using FWTL.Common.Extensions;
using FWTL.Core.Commands;
using FWTL.Core.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.RabbitMq
{
    public class RequestDispatcher : ICommandDispatcher
    {
        private readonly IClientFactory _clientFactory;
        private readonly IServiceProvider _context;
        private readonly RequestToCommandMapper _requestToCommandMapper;
        private readonly IGuidService _guidService;

        public RequestDispatcher(
            IServiceProvider context,
            IClientFactory clientFactory,
            RequestToCommandMapper requestToCommandMapper,
            IGuidService guidService)
        {
            _context = context;
            _clientFactory = clientFactory;
            _requestToCommandMapper = requestToCommandMapper;
            _guidService = guidService;
        }

        public async Task<Guid> DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand
        {
            IValidator<TCommand> validator = _context.GetService<IValidator<TCommand>>();
            if (validator.IsNotNull())
            {
                ValidationResult validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }

            command.CorrelationId = _guidService.New;

            IRequestClient<TCommand> client = _clientFactory.CreateRequestClient<TCommand>(new Uri("queue:commands"), TimeSpan.FromMinutes(5));
            Response<FWTL.Common.Cqrs.Responses.Response> response = await client.GetResponse<FWTL.Common.Cqrs.Responses.Response>(command);

            if (response.Message.StatusCode == HttpStatusCode.BadRequest)
            {
                //TODO
                throw new AppValidationException(Guid.Empty, response.Message.Errors);
            }

            if (response.Message.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new InvalidOperationException(response.Message.Id.ToString());
            }

            return response.Message.Id;
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