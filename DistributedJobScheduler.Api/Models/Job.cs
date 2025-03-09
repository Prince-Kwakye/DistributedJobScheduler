using System;
using System.Collections.Generic;

namespace DistributedJobScheduler.Api.Models
{
    public class Job
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Job primary key
        public string Name { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public int Priority { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int RetryCount { get; set; } = 0;
        public int MaxRetries { get; set; } = 3;
        public string? ErrorMessage { get; set; }

        // Self-referencing relationship
        public Guid? DependentOnJobId { get; set; }
        public Job? DependentOnJob { get; set; }
        public ICollection<Job> DependentJobs { get; set; } = [];

        // ✅ Ensure UserId matches ApplicationUser.Id type (string)
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }


    public enum JobStatus
    {
        Pending,    // Job is waiting to be processed
        Processing, // Job is currently being processed
        Completed,  // Job has been successfully completed
        Failed      // Job failed and may be retried
    }
}
