using NodaTime;
using System;
using System.Collections.Generic;

namespace FWTL.Aggregate
{
    public class Job
    {
        public Guid Id { get; set; }

        public string DialogId { get; set; }

        public long MaxHistoryId { get; set; }

        public JobStatus JobStatus { get; set; }

        public Instant CreatedAt { get; set; }

        public virtual ICollection<TelegramAccountJob> TelegramAccountJobs { get; set; } = new List<TelegramAccountJob>();
    }

    public enum JobStatus
    {
        Created = 1,
    }
}