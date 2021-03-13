using System;
using System.Net;

namespace FWTL.Common.Commands
{
    public class ErrorResponse : Response
    {
        public ErrorResponse(Guid exceptionId)
        {
            Id = exceptionId;
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }
}