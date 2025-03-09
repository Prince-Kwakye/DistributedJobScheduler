using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistributedJobScheduler.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DistributedJobScheduler.Api.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        private static User MapToUser(ApplicationUser appUser)
        {
            return new User
            {
                Id = appUser.Id, // No need for Guid conversion since User.Id is string
                UserName = appUser.UserName ?? string.Empty,
                Email = appUser.Email ?? string.Empty
            };
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());
            return appUser != null ? MapToUser(appUser) : null;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            return appUser != null ? MapToUser(appUser) : null;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(MapToUser);
        }

        public async Task AddAsync(User user)
        {
            var appUser = new ApplicationUser
            {
                Id = user.Id ?? Guid.NewGuid().ToString(), // Ensure ID is not null
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };

            await _context.Users.AddAsync(appUser);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            if (appUser != null)
            {
                appUser.UserName = user.UserName ?? string.Empty;
                appUser.Email = user.Email ?? string.Empty;
                _context.Users.Update(appUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());
            if (appUser != null)
            {
                _context.Users.Remove(appUser);
                await _context.SaveChangesAsync();
            }
        }
    }
}
