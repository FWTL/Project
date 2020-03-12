using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace FWTL.Common.Commands
{
    public class Response
    {
        public Guid Id { get; set; }

        public IEnumerable<ValidationFailure> Errors { get; set; } = new List<ValidationFailure>();

        public Response()
        {
        }

        //public Response(IAggregateRoot aggregateRoot)
        //{
        //    Id = aggregateRoot.Id;
        //}

        public Response(ValidationException validationException)
        {
            Errors = validationException.Errors;
        }
    }
}