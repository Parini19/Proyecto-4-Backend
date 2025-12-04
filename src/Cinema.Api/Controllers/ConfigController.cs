using Microsoft.AspNetCore.Mvc;
using Cinema.Api.Services;
using Serilog;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        private readonly RuntimeConfigService _configService;

        public ConfigController(RuntimeConfigService configService)
        {
            _configService = configService;
        }

        /// <summary>
        /// Get current audit logging status
        /// GET /api/config/audit-logging
        /// </summary>
        [HttpGet("audit-logging")]
        public IActionResult GetAuditLoggingStatus()
        {
            return Ok(new
            {
                success = true,
                enabled = _configService.IsAuditLoggingEnabled,
                message = _configService.IsAuditLoggingEnabled
                    ? "Audit logging is currently ENABLED"
                    : "Audit logging is currently DISABLED"
            });
        }

        /// <summary>
        /// Enable or disable audit logging at runtime
        /// POST /api/config/audit-logging
        /// Body: { "enabled": true }
        /// </summary>
        [HttpPost("audit-logging")]
        public IActionResult SetAuditLoggingStatus([FromBody] AuditLoggingRequest request)
        {
            try
            {
                _configService.SetAuditLogging(request.Enabled);

                var action = request.Enabled ? "ENABLED" : "DISABLED";
                Log.Warning("🔧 Audit logging has been {Action} via admin panel", action);

                return Ok(new
                {
                    success = true,
                    enabled = request.Enabled,
                    message = $"Audit logging has been {action}",
                    note = request.Enabled
                        ? "⚠️ All user actions will now be logged to Firestore (increases DB usage)"
                        : "✅ Audit logging disabled - no logs will be saved (recommended for testing)"
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating audit logging status");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error updating audit logging: {ex.Message}"
                });
            }
        }
    }

    public class AuditLoggingRequest
    {
        public bool Enabled { get; set; }
    }
}
