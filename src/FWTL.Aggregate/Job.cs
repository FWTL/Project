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

        public long MessagesToProcess { get; set; }

        public long ProcessedMessages { get; set; }

        public JobStatus JobStatus { get; set; }

        public Instant CreatedAt { get; set; }

        public virtual ICollection<AccountJob> TelegramAccountJobs { get; set; } = new List<AccountJob>();
    }

    public enum JobStatus
    {
        Started = 1,
    }
}