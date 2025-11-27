using System;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly FirestoreAuditLogService _auditLogService;

        public AuditLogController(FirestoreAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAuditLog([FromBody] AuditLog log)
        {
            try
            {
                // Get IP address from request
                log.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                await _auditLogService.AddAuditLogAsync(log);
                return Ok(new { success = true, id = log.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to add audit log.", error = ex.Message });
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetAuditLog(string id)
        {
            try
            {
                var log = await _auditLogService.GetAuditLogAsync(id);
                if (log == null)
                    return NotFound(new { success = false, message = "Audit log not found." });

                return Ok(new { success = true, log });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get audit log.", error = ex.Message });
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllAuditLogs()
        {
            try
            {
                var logs = await _auditLogService.GetAllAuditLogsAsync();
                return Ok(new { success = true, logs, count = logs.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get audit logs.", error = ex.Message });
            }
        }

        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetAuditLogsByUser(string userId)
        {
            try
            {
                var logs = await _auditLogService.GetAuditLogsByUserAsync(userId);
                return Ok(new { success = true, logs, count = logs.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get audit logs by user.", error = ex.Message });
            }
        }

        [HttpGet("get-by-entity")]
        public async Task<IActionResult> GetAuditLogsByEntity([FromQuery] string entityType, [FromQuery] string entityId)
        {
            try
            {
                var logs = await _auditLogService.GetAuditLogsByEntityAsync(entityType, entityId);
                return Ok(new { success = true, logs, count = logs.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get audit logs by entity.", error = ex.Message });
            }
        }

        [HttpGet("get-by-action/{action}")]
        public async Task<IActionResult> GetAuditLogsByAction(string action)
        {
            try
            {
                var logs = await _auditLogService.GetAuditLogsByActionAsync(action);
                return Ok(new { success = true, logs, count = logs.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get audit logs by action.", error = ex.Message });
            }
        }

        [HttpGet("get-by-date-range")]
        public async Task<IActionResult> GetAuditLogsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var logs = await _auditLogService.GetAuditLogsByDateRangeAsync(startDate, endDate);
                return Ok(new { success = true, logs, count = logs.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get audit logs by date range.", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAuditLog(string id)
        {
            try
            {
                await _auditLogService.DeleteAuditLogAsync(id);
                return Ok(new { success = true, message = $"Audit log {id} deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to delete audit log.", error = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetAuditLogCount()
        {
            try
            {
                var count = await _auditLogService.GetAuditLogCountAsync();
                return Ok(new { success = true, count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get audit log count.", error = ex.Message });
            }
        }

        /// <summary>
        /// Seed some example audit logs for testing
        /// </summary>
        [HttpPost("seed")]
        public async Task<IActionResult> SeedAuditLogs([FromQuery] int count = 20)
        {
            try
            {
                var random = new Random();
                var actions = new[] { "CREATE", "UPDATE", "DELETE", "VIEW", "LOGIN", "LOGOUT" };
                var entities = new[] { "Movie", "Booking", "User", "Screening", "TheaterRoom", "FoodCombo" };
                var severities = new[] { "Info", "Warning", "Error", "Critical" };
                var users = new[] { "admin@cinema.com", "manager@cinema.com", "staff@cinema.com" };

                for (int i = 0; i < count; i++)
                {
                    var log = new AuditLog
                    {
                        Action = actions[random.Next(actions.Length)],
                        EntityType = entities[random.Next(entities.Length)],
                        EntityId = Guid.NewGuid().ToString(),
                        UserId = Guid.NewGuid().ToString(),
                        UserEmail = users[random.Next(users.Length)],
                        Description = $"Test audit log entry #{i + 1}",
                        Timestamp = DateTime.UtcNow.AddDays(-random.Next(30)),
                        IpAddress = $"192.168.1.{random.Next(1, 255)}",
                        Details = $"Additional details for entry #{i + 1}",
                        Severity = severities[random.Next(severities.Length)]
                    };

                    await _auditLogService.AddAuditLogAsync(log);
                }

                return Ok(new { success = true, message = $"Created {count} audit logs", count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error seeding audit logs: {ex.Message}" });
            }
        }
    }
}
