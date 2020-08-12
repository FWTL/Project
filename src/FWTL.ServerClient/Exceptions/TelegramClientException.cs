using System;
using System.Collections.Generic;
using FWTL.TelegramClient.Responses;

namespace FWTL.TelegramClient.Exceptions
{
    class TelegramClientException : Exception
    {
        private readonly IEnumerable<Error> _errors;

        public TelegramClientException(IEnumerable<Error> errors)
        {
            _errors = errors;
        }
    }
}
