using System;
using System.Collections.Generic;
using System.Net;
using FluentValidation.Results;

namespace FWTL.Common.Commands
{
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