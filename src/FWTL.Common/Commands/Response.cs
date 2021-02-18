using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation;
using FluentValidation.Results;
using FWTL.TelegramClient.Exceptions;

namespace FWTL.Common.Commands
{
    public class BadRequestResponse : Response
    {
        public BadRequestResponse(ValidationException validationException)
        {
            Errors = validationException.Errors;
            StatusCode = HttpStatusCode.BadRequest;
        }

        public BadRequestResponse(TelegramClientException telegramClientException)
        {
            Errors = telegramClientException.Errors.Select(error => new ValidationFailure("Telegram", error.Message));
            StatusCode = HttpStatusCode.BadRequest;
        }
    }

    public class ErrorResponse : Response
    {
        public ErrorResponse(Guid exceptionId)
        {
            Id = exceptionId;
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }

    public class Response
    {
        public Response()
        {
        }

        public Response(Guid id)
        {
            Id = id;
            StatusCode = HttpStatusCode.OK;
        }

        public Guid Id { get; set; }

        public IEnumerable<ValidationFailure> Errors { get; set; } = new List<ValidationFailure>();

        public HttpStatusCode StatusCode { get; set; }
    }

    public class Response<TResult> : Response
    {
        public Response()
        {
        }

        public Response(Guid id, TResult result) : base(id)
        {
            Result = result;
        }

        public TResult Result { get; set; }
    }
}