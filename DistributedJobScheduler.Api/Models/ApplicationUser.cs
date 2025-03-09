using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DistributedJobScheduler.Api.Models
{
    public class ApplicationUser : IdentityUser  // Id is already a string
    {
        public List<Job> Jobs { get; set; } = [];
    }
}