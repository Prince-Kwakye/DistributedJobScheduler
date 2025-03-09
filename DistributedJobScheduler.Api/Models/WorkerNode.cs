using System;

namespace DistributedJobScheduler.Api.Models
{
    public class WorkerNode
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for the worker node
        public string? Name { get; set; } // Name or identifier for the worker
        public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow; // Timestamp of the last heartbeat
        public bool IsActive { get; set; } = true; // Indicates if the worker is active
        public int HealthCheckIntervalSeconds { get; set; } = 30; // Interval for health checks
    }
}
