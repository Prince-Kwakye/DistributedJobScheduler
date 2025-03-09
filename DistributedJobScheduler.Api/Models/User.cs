using DistributedJobScheduler.Api.Models;
using System;
using System.Collections.Generic;

namespace DistributedJobScheduler.Api.Models
{
    public class User
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString(); // Unique identifier for the user
        public string? UserName { get; set; } // Username for authentication
        public string? Email { get; set; } // User's email address
        public string? PasswordHash { get; set; } // Hashed password for security
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp when the user was created
        public ICollection<Job> Jobs { get; set; } = []; // Jobs submitted by the user
    }
}
