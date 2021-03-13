using System.Linq;
using System.Net;
using FluentValidation;
using FluentValidation.Results;
using FWTL.Common.Exceptions;

namespace FWTL.Common.Cqrs.Responses
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
}