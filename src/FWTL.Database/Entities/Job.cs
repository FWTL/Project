using System;
using System.Collections.Generic;
using NodaTime;

namespace FWTL.Database.Entities
{
    public class Job
    {
        public Guid Id { get; set; }

        public string DialogId { get; set; }

        public long MaxHistoryId { get; set; }

        public long MessagesToProcess { get; set; }

        public long ProcessedMessages { get; set; }

        public JobStatuses JobStatus { get; set; }

        public Instant CreatedAt { get; set; }

        public virtual ICollection<AccountJob> AccountJobs { get; set; } = new List<AccountJob>();
    }

    public enum JobStatuses
    {
        Canceled = -2,
        Failed = -1,
        InProgress = 1,
        Completed = 1,
    }
}