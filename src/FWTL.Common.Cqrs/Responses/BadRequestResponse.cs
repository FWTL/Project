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
}