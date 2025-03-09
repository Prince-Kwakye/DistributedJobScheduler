using DistributedJobScheduler.Api.Models;
using System;
using System.Threading.Tasks;

namespace DistributedJobScheduler.Api.Repositories
{
    public interface IWorkerNodeRepository
    {
        Task<WorkerNode?> GetByIdAsync(Guid id);
        Task AddAsync(WorkerNode workerNode);
        Task UpdateAsync(WorkerNode workerNode);
        Task DeleteAsync(Guid id);
    }
}