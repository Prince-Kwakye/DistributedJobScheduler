using DistributedJobScheduler.Api.Repositories;
using DistributedJobScheduler.Api.Models;
using System;
using System.Threading.Tasks;

namespace DistributedJobScheduler.Api.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<User?> CreateUserAsync(string username, string email, string passwordHash)
        {
            var user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = passwordHash
            };

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<User?> GetUserAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<bool> UpdateUserAsync(Guid id, string username, string email, string passwordHash)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            user.UserName = username;
            user.Email = email;
            user.PasswordHash = passwordHash;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            await _userRepository.DeleteAsync(id);
            return true;
        }
    }
}