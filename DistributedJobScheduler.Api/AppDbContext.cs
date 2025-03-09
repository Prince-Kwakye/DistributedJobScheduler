using DistributedJobScheduler.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DistributedJobScheduler.Api
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Job> Jobs { get; set; }
        public DbSet<WorkerNode> WorkerNodes { get; set; }
        public DbSet<JobResult> JobResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Self-referencing relationship for Job
            modelBuilder.Entity<Job>()
                .HasOne(j => j.DependentOnJob)
                .WithMany(j => j.DependentJobs)
                .HasForeignKey(j => j.DependentOnJobId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Fix Job-User Relationship (UserId should be a string)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.User)
                .WithMany(u => u.Jobs)
                .HasForeignKey(j => j.UserId) // ✅ Explicitly define the FK
                .HasPrincipalKey(u => u.Id); // ✅ Ensures it matches ApplicationUser.Id
            ;

            // Configure Job-JobResult Relationship
            modelBuilder.Entity<JobResult>()
                .HasOne(jr => jr.Job)
                .WithMany()
                .HasForeignKey(jr => jr.JobId);
        }
    }

    // ✅ Fix ApplicationUser (keep Id as string, which is the default in IdentityUser)
    public class ApplicationUser : IdentityUser
    {
        public List<Job> Jobs { get; set; } = [];
    }
}
