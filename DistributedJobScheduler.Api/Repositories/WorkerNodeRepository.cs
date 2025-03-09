using DistributedJobScheduler.Api.Models;

namespace DistributedJobScheduler.Api.Repositories
{
    public class WorkerNodeRepository(AppDbContext context) : IWorkerNodeRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<WorkerNode?> GetByIdAsync(Guid id)
        {
            return await _context.WorkerNodes.FindAsync(id);
        }


        public async Task AddAsync(WorkerNode workerNode)
        {
            await _context.WorkerNodes.AddAsync(workerNode);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkerNode workerNode)
        {
            _context.WorkerNodes.Update(workerNode);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var workerNode = await _context.WorkerNodes.FindAsync(id);
            if (workerNode != null)
            {
                _context.WorkerNodes.Remove(workerNode);
                await _context.SaveChangesAsync();
            }
        }
    }
}