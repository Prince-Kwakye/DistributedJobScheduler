using System;
using System.Threading.Tasks;
using DistributedJobScheduler.Api.Models;
using DistributedJobScheduler.Api.Services;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace DistributedJobScheduler.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkerNodeController(WorkerNodeService workerNodeService, IConnectionMultiplexer redis) : ControllerBase
    {
        private readonly WorkerNodeService _workerNodeService = workerNodeService;
        private readonly IConnectionMultiplexer _redis = redis;

        // GET: api/workernode/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<WorkerNode>> GetWorkerNode(Guid id)
        {
            var db = _redis.GetDatabase();
            var cachedWorker = await db.StringGetAsync($"workernode:{id}");

            if (cachedWorker.HasValue && !string.IsNullOrEmpty(cachedWorker))
            {
                var workerNode = JsonSerializer.Deserialize<WorkerNode>(cachedWorker.ToString());
                if (workerNode != null)
                {
                    return Ok(workerNode);
                }
            }

            var workerNodeFromDb = await _workerNodeService.GetWorkerNodeAsync(id);
            if (workerNodeFromDb == null)
            {
                return NotFound();
            }

            string json = JsonSerializer.Serialize(workerNodeFromDb);
            if (!string.IsNullOrEmpty(json))
            {
                await db.StringSetAsync($"workernode:{id}", json, TimeSpan.FromMinutes(5));
            }

            return Ok(workerNodeFromDb);
        }

        // POST: api/workernode
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WorkerNode>> CreateWorkerNode([FromBody] WorkerNodeCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Worker node name is required.");
            }

            var workerNode = await _workerNodeService.CreateWorkerNodeAsync(request.Name, request.HealthCheckIntervalSeconds);
            if (workerNode == null)
            {
                return StatusCode(500, "Error creating worker node.");
            }

            var pubSub = _redis.GetSubscriber();
            string message = JsonSerializer.Serialize(workerNode);
            await pubSub.PublishAsync(RedisChannel.Literal("workernode:created"), message);

            return CreatedAtAction(nameof(GetWorkerNode), new { id = workerNode.Id }, workerNode);
        }

        // PUT: api/workernode/{id}/heartbeat
        [HttpPut("{id}/heartbeat")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> UpdateWorkerNodeHeartbeat(Guid id)
        {
            bool updated = await _workerNodeService.UpdateWorkerHeartbeatAsync(id);
            if (!updated)
            {
                return NotFound("Worker node not found.");
            }

            var pubSub = _redis.GetSubscriber();
            await pubSub.PublishAsync(RedisChannel.Literal("workernode:heartbeat"), id.ToString());

            return NoContent();
        }

        // DELETE: api/workernode/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteWorkerNode(Guid id)
        {
            bool result = await _workerNodeService.DeleteWorkerNodeAsync(id);
            if (!result)
            {
                return NotFound("Worker node not found.");
            }

            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync($"workernode:{id}");

            return NoContent();
        }
    }

    public class WorkerNodeCreateRequest
    {
        public string Name { get; set; } = string.Empty;  // Ensures Name is never null
        public int HealthCheckIntervalSeconds { get; set; } = 30;
    }
}
