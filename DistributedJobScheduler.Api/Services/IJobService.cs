using DistributedJobScheduler.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DistributedJobScheduler.Api.Services
{
    public interface IJobService
    {
        Task<Job?> CreateJobAsync(string name, string payload, int priority, Guid userId, Guid? dependentOnJobId = null);
        Task<Job?> ProcessJobAsync(Guid id);
        Task<Job?> GetJobAsync(Guid id);
        Task<IEnumerable<Job?>> GetPendingJobsAsync();
        Task<Job?> GetNextJobAsync();
        Task<bool> DeleteJobAsync(Guid id);
    }
}