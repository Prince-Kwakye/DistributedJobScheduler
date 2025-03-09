using DistributedJobScheduler.Api.Models;
using System;
using System.Threading.Tasks;

namespace DistributedJobScheduler.Api.Services
{
    public interface IUserService
    {
        Task<User?> CreateUserAsync(string username, string email, string passwordHash);
        Task<User?> GetUserAsync(Guid id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UpdateUserAsync(Guid id, string username, string email, string passwordHash);
        Task<bool> DeleteUserAsync(Guid id);
    }
}