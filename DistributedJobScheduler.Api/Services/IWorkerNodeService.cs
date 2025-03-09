using DistributedJobScheduler.Api.Models;
using System;
using System.Threading.Tasks;

namespace DistributedJobScheduler.Api.Services
{
    public interface IWorkerNodeService
    {
        Task<WorkerNode?> CreateWorkerNodeAsync(string name, int healthCheckIntervalSeconds);
        Task<WorkerNode?> GetWorkerNodeAsync(Guid id);
        Task<bool> UpdateWorkerHeartbeatAsync(Guid id);
        Task<bool> DeleteWorkerNodeAsync(Guid id);
    }
}