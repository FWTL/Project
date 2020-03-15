using System;
using System.Collections.Generic;
using FluentValidation;
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
        }

        public Response(ValidationException validationException)
        {
            Errors = validationException.Errors;
        }

        public Guid Id { get; set; }

        public IEnumerable<ValidationFailure> Errors { get; set; } = new List<ValidationFailure>();
    }
}