using System;
using System.Threading.Tasks;
using FluentValidation;
using FWTL.Common.Commands;
using FWTL.Core.Helpers;
using FWTL.Core.Queries;
using FWTL.TelegramClient.Exceptions;
using MassTransit;

namespace FWTL.RabbitMq
{
    public class QueryConsumer<TQuery, TResult> : IConsumer<TQuery> where TQuery : class, IQuery
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;
        private readonly IExceptionHandler _exceptionHandler;

        public QueryConsumer(IQueryHandler<TQuery, TResult> handler, IExceptionHandler exceptionHandler)
        {
            _handler = handler;
            _exceptionHandler = exceptionHandler;
        }

        public async Task Consume(ConsumeContext<TQuery> context)
        {
            try
            {
                var result = await _handler.HandleAsync(context.Message);
                await context.RespondAsync(new Common.Commands.Response<TResult>(context.RequestId.Value, result));
            }
            catch (ValidationException ex)
            {
                await context.RespondAsync(new Response(ex));
            }
            catch (TelegramClientException ex)
            {
                await context.RespondAsync(new Response(ex));
            }
            catch (Exception ex)
            {
                var exceptionId = _exceptionHandler.Handle(ex, context.Message);
                await context.RespondAsync(new Response(exceptionId));
            }
        }
    }
}