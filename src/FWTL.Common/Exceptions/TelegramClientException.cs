using System;
using System.Collections.Generic;
using FWTL.Core.Services.Telegram.Dto;

namespace FWTL.Common.Exceptions
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