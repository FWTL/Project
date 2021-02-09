using System;
using System.Collections.Generic;
using FWTL.TelegramClient.Responses;

namespace FWTL.TelegramClient.Exceptions
{
    public class TelegramClientException : Exception
    {
        public IEnumerable<Error> Errors { get; }

        public TelegramClientException(IEnumerable<Error> errors)
        {
            Errors = errors;
        }
    }
}
