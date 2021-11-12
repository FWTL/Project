using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace FWTL.Core.Events
{
    public class ValidationExceptionRaised : IEvent
    {
        public ValidationExceptionRaised(Guid aggregateId, IEnumerable<ValidationFailure> errors)
        {
            AggregateId = aggregateId;
            Errors = errors;
        }

        public Guid AggregateId { get; set; }

        public IEnumerable<ValidationFailure> Errors { get; set; }

        public Guid CorrelationId { get; set; }
    }
}