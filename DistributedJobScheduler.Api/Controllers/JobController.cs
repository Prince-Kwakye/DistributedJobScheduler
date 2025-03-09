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
    public class JobController(IJobService jobService, IConnectionMultiplexer redis) : ControllerBase
    {
        private readonly IJobService _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        private readonly IConnectionMultiplexer _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // GET: api/job/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<Job>> GetJob(Guid id)
        {
            var db = _redis.GetDatabase();
            var cachedJob = await db.StringGetAsync($"job:{id}");

            if (!cachedJob.IsNullOrEmpty)
            {
                var jobFromCache = JsonSerializer.Deserialize<Job>(cachedJob!, _jsonOptions);
                if (jobFromCache != null)
                {
                    return Ok(jobFromCache);
                }
            }

            var job = await _jobService.GetJobAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            await db.StringSetAsync($"job:{id}", JsonSerializer.Serialize(job, _jsonOptions), TimeSpan.FromMinutes(5));
            return Ok(job);
        }

        // POST: api/job
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Job>> CreateJob([FromBody] JobCreateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Payload))
            {
                return BadRequest("Name and Payload are required.");
            }

            var job = await _jobService.CreateJobAsync(request.Name, request.Payload, request.Priority, request.UserId, request.DependentOnJobId);
            if (job == null)
            {
                return StatusCode(500, "Job creation failed.");
            }

            var pubSub = _redis.GetSubscriber();
            await pubSub.PublishAsync(RedisChannel.Literal("job:created"), JsonSerializer.Serialize(job, _jsonOptions));

            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }

        // PUT: api/job/{id}/process
        [HttpPut("{id}/process")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Job>> ProcessJob(Guid id)
        {
            try
            {
                var job = await _jobService.ProcessJobAsync(id);
                var pubSub = _redis.GetSubscriber();
                await pubSub.PublishAsync(RedisChannel.Literal("job:updated"), JsonSerializer.Serialize(job, _jsonOptions));
                return Ok(job);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/job/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteJob(Guid id)
        {
            var result = await _jobService.DeleteJobAsync(id);
            if (!result)
            {
                return NotFound();
            }

            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync($"job:{id}");
            return NoContent();
        }
    }

    public class JobCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public int Priority { get; set; }
        public Guid UserId { get; set; }
        public Guid? DependentOnJobId { get; set; }
    }
}
