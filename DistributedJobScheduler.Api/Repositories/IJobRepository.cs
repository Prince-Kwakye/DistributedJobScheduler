using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DistributedJobScheduler.Api.Models;

namespace DistributedJobScheduler.Api.Repositories
{
    public interface IJobRepository
    {
        Task<Job?> GetByIdAsync(Guid id);
        Task<IEnumerable<Job>> GetAllAsync();
        Task AddAsync(Job job);
        Task UpdateAsync(Job job);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Job>> GetPendingJobsAsync();
        Task<Job?> GetNextJobAsync();
    }
}