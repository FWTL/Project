using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FWTL.TelegramClient.Exceptions;

namespace FWTL.Common.Commands
{
    public class Response
    {
        public Response()
        {
        }

        public Response(Guid id, HttpStatusCode statusCode)
        {
            Id = id;
            StatusCode = statusCode;
        }

        public Response(Guid id)
        {
            Id = id;
            StatusCode = HttpStatusCode.OK;
        }

        public Response(ValidationException validationException)
        {
            Errors = validationException.Errors;
            StatusCode = HttpStatusCode.BadRequest;
        }

        public Response(TelegramClientException telegramClientException)
        {
            Errors = telegramClientException.Errors.Select(error => new ValidationFailure("Telegram", error.Message));
            StatusCode = HttpStatusCode.BadRequest;
        }

        public Guid Id { get; set; }

        public IEnumerable<ValidationFailure> Errors { get; set; } = new List<ValidationFailure>();

        public HttpStatusCode StatusCode { get; set; }
    }

    public class Response<TResult> : Response
    {
        public Response(Guid id, TResult result) : base(id)
        {
            Result = result;
        }

        public TResult Result { get; set; }
    }
}