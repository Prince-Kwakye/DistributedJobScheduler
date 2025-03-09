using System;
using System.Text.Json;
using System.Threading.Tasks;
using DistributedJobScheduler.Api.Models;
using DistributedJobScheduler.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DistributedJobScheduler.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(UserService userService, IConnectionMultiplexer redis) : ControllerBase
    {
        private readonly UserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private readonly IConnectionMultiplexer _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        // GET: api/user/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var db = _redis.GetDatabase();
            var cachedUser = await db.StringGetAsync($"user:{id}");

            if (!cachedUser.IsNullOrEmpty)
            {
                var userFromCache = JsonSerializer.Deserialize<User>(cachedUser!, _jsonOptions);
                if (userFromCache is not null)
                {
                    return Ok(userFromCache);
                }
            }

            // Fetch from PostgreSQL if not found in cache
            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Cache the user in Redis
            await db.StringSetAsync($"user:{id}", JsonSerializer.Serialize(user, _jsonOptions), TimeSpan.FromMinutes(5));

            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserCreateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.PasswordHash))
            {
                return BadRequest("Username, Email, and Password are required.");
            }

            var user = await _userService.CreateUserAsync(request.Username, request.Email, request.PasswordHash);
            if (user == null)
            {
                return StatusCode(500, "User creation failed.");
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest("Username and Email are required.");
            }

            var passwordHash = request.PasswordHash ?? string.Empty; // Ensure passwordHash is never null

            bool updateSuccess = await _userService.UpdateUserAsync(id, request.Username, request.Email, passwordHash);
            if (!updateSuccess)
            {
                return NotFound();
            }

            // Retrieve the updated user from the database
            var updatedUser = await _userService.GetUserAsync(id);

            // Update Redis cache
            var db = _redis.GetDatabase();
            await db.StringSetAsync($"user:{id}", JsonSerializer.Serialize(updatedUser, _jsonOptions), TimeSpan.FromMinutes(5));

            return Ok(updatedUser);
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound();
            }

            // Remove user from Redis cache
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync($"user:{id}");

            return NoContent();
        }
    }

    public class UserCreateRequest
    {
        public string Username { get; set; } = string.Empty; // Ensure non-nullable
        public string Email { get; set; } = string.Empty; // Ensure non-nullable
        public string PasswordHash { get; set; } = string.Empty; // Ensure non-nullable
    }

    public class UserUpdateRequest
    {
        public string Username { get; set; } = string.Empty; // Ensure non-nullable
        public string Email { get; set; } = string.Empty; // Ensure non-nullable
        public string? PasswordHash { get; set; } // Allow null for password update
    }
}
