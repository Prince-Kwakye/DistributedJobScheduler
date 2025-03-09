using DistributedJobScheduler.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedJobScheduler.Api.Repositories
{
    public class JobRepository(AppDbContext context) : IJobRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Job?> GetByIdAsync(Guid id)
        {
            return await _context.Jobs
                .Include(j => j.DependentOnJob) // Include the dependent job
                .AsNoTracking() // Optimize performance for read-only queries
                .FirstOrDefaultAsync(j => j.Id == id); // ✅ FIXED: Compare Guid to Guid
        }

        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            return await _context.Jobs
                .Include(j => j.DependentOnJob)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Job job)
        {
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Job job)
        {
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var job = await _context.Jobs.FindAsync(id); // ✅ FIXED: Use Guid directly
            if (job != null)
            {
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Job>> GetPendingJobsAsync()
        {
            return await _context.Jobs
                .Where(j => j.Status == JobStatus.Pending)
                .Include(j => j.DependentOnJob)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Job?> GetNextJobAsync()
        {
            return await _context.Jobs
                .Include(j => j.DependentOnJob)
                .Where(j => j.Status == JobStatus.Pending &&
                            (j.DependentOnJobId == null || j.DependentOnJob!.Status == JobStatus.Completed))
                .OrderBy(j => j.Priority)
                .ThenBy(j => j.CreatedAt)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
