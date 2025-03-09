using System;

namespace DistributedJobScheduler.Api.Models
{
    public class JobResult
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for the result
        public Guid JobId { get; set; } // Foreign Key reference to the Job
        public Job? Job { get; set; } //  Navigation property for EF Core relationship
        public JobStatus Status { get; set; } // Final status of the job
        public string? Output { get; set; } // Output or result of the job
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow; // Timestamp when the job was completed
    }
}
