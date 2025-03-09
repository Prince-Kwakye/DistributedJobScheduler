using DistributedJobScheduler.Api.Models;
using DistributedJobScheduler.Api.Repositories;
using System;
using System.Threading.Tasks;

namespace DistributedJobScheduler.Api.Services
{
    public class WorkerNodeService(IWorkerNodeRepository workerNodeRepository) : IWorkerNodeService
    {
        private readonly IWorkerNodeRepository _workerNodeRepository = workerNodeRepository;

        public async Task<WorkerNode?> CreateWorkerNodeAsync(string name, int healthCheckIntervalSeconds)
        {
            var workerNode = new WorkerNode
            {
                Name = name,
                HealthCheckIntervalSeconds = healthCheckIntervalSeconds
            };

            await _workerNodeRepository.AddAsync(workerNode);
            return workerNode;
        }

        public async Task<WorkerNode?> GetWorkerNodeAsync(Guid id)
        {
            return await _workerNodeRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateWorkerHeartbeatAsync(Guid id)
        {
            var workerNode = await _workerNodeRepository.GetByIdAsync(id);
            if (workerNode == null)
            {
                return false;
            }

            workerNode.LastHeartbeat = DateTime.UtcNow;
            await _workerNodeRepository.UpdateAsync(workerNode);
            return true;
        }

        public async Task<bool> DeleteWorkerNodeAsync(Guid id)
        {
            var workerNode = await _workerNodeRepository.GetByIdAsync(id);
            if (workerNode == null)
            {
                return false;
            }

            await _workerNodeRepository.DeleteAsync(id);
            return true;
        }
    }
}