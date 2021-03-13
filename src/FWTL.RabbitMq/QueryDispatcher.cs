using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Extensions;
using FWTL.Common.Services;
using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Core.Validation;
using FWTL.Domain.Traits;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.RabbitMq
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IClientFactory _clientFactory;
        private readonly IServiceProvider _context;
        private readonly RequestToQueryMapper _requestToQueryMapper;

        public QueryDispatcher(
            IServiceProvider context,
            IClientFactory clientFactory,
            RequestToQueryMapper requestToQueryMapper)
        {
            _context = context;
            _clientFactory = clientFactory;
            _requestToQueryMapper = requestToQueryMapper;
        }

        public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query)
            where TQuery : class, IQuery
        {
            await TraitValidationAsync<TQuery, IPagingTrait>(query);

            var validator = _context.GetService<IValidator<TQuery>>();
            if (validator.IsNotNull())
            {
                var validationResult = await validator.ValidateAsync(query);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }

            var client = _clientFactory.CreateRequestClient<TQuery>(new Uri("queue:queries"), TimeSpan.FromMinutes(10));
            var response = await client.GetResponse<Common.Commands.Response<TResult>>(query);

            if (response.Message.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new AppValidationException(response.Message.Errors);
            }

            if (response.Message.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new InvalidOperationException(response.Message.Id.ToString());
            }

            return response.Message.Result;
        }

        public async Task<TResult> DispatchAsync<TRequest, TQuery, TResult>(TRequest request)
            where TQuery : class, IQuery
            where TRequest : class, IRequest
        {
            var query = _requestToQueryMapper.Map<TRequest, TQuery>(request);
            return await DispatchAsync<TQuery, TResult>(query);
        }

        public async Task<TResult> DispatchAsync<TRequest, TQuery, TResult>(TRequest request, Action<TQuery> afterMap)
            where TQuery : class, IQuery
            where TRequest : class, IRequest
        {
            var query = _requestToQueryMapper.Map(request, afterMap);
            return await DispatchAsync<TQuery, TResult>(query);
        }

        public async Task TraitValidationAsync<TQuery, TTraitValidator>(TQuery query)
            where TQuery : class, IQuery
        {
            if (query is TTraitValidator)
            {
                var pagingValidator = _context.GetService<IValidator<TTraitValidator>>();
                var pagingValidatorResult = await pagingValidator.ValidateAsync(query);
                if (!pagingValidatorResult.IsValid)
                {
                    throw new ValidationException(pagingValidatorResult.Errors);
                }
            }
        }
    }
}