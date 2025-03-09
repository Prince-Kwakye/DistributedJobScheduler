using DistributedJobScheduler.Api.Models;
using DistributedJobScheduler.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedJobScheduler.Api.Services
{
    public class JobService(IJobRepository jobRepository) : IJobService
    {
        private readonly IJobRepository _jobRepository = jobRepository;

        public async Task<Job?> CreateJobAsync(string name, string payload, int priority, Guid userId, Guid? dependentOnJobId = null)
        {
            var job = new Job
            {
                Name = name,
                Payload = payload,
                Priority = priority,
                UserId = userId.ToString(), // ✅ FIXED: No .ToString() needed
                DependentOnJobId = dependentOnJobId
            };

            await _jobRepository.AddAsync(job);
            return job;
        }

        public async Task<Job?> ProcessJobAsync(Guid id)
        {
            var job = await _jobRepository.GetByIdAsync(id) ?? throw new ArgumentException("Job not found");
            if (job.DependentOnJobId.HasValue && job.DependentOnJob?.Status != JobStatus.Completed)
            {
                throw new InvalidOperationException("Dependent job has not been completed");
            }

            job.Status = JobStatus.Processing;
            job.StartedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job);

            // Simulate job processing
            await Task.Delay(5000); // Simulate a 5-second job

            job.Status = JobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job);

            return job;
        }

        public async Task<Job?> GetJobAsync(Guid id)
        {
            return await _jobRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Job?>> GetPendingJobsAsync()
        {
            return await _jobRepository.GetPendingJobsAsync();
        }

        public async Task<Job?> GetNextJobAsync()
        {
            return await _jobRepository.GetNextJobAsync();
        }

        public async Task<bool> DeleteJobAsync(Guid id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return false;
            }

            await _jobRepository.DeleteAsync(id);
            return true;
        }
    }
}
