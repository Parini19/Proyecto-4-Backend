using Serilog;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using Microsoft.FeatureManagement;

namespace Cinema.Api.Utilities;

public class UserActionAuditMiddleware
{
    private readonly RequestDelegate _next;

    public UserActionAuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, FirestoreAuditLogService auditLogService, IFeatureManager featureManager)
    {
        var userId = context.User?.FindFirst("user_id")?.Value
                     ?? context.User?.FindFirst("uid")?.Value
                     ?? "anonymous";

        var userEmail = context.User?.FindFirst("email")?.Value
                        ?? context.User?.FindFirst("preferred_username")?.Value
                        ?? "anonymous@system.com";

        var path = context.Request.Path.Value;
        var method = context.Request.Method;
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        try
        {
            await _next(context);
            var statusCode = context.Response.StatusCode;

            // Log to Serilog
            Log.Information("AUDIT {Method} {Path} {StatusCode} by {UserId} from {IP}", method, path, statusCode, userId, ip);

            // Check if audit logging feature is enabled
            var auditEnabled = await featureManager.IsEnabledAsync("AuditLogging");

            // Save to Firestore if feature is enabled and it's a write operation (POST, PUT, DELETE) and successful
            if (auditEnabled &&
                (method == "POST" || method == "PUT" || method == "DELETE") &&
                statusCode >= 200 && statusCode < 300 &&
                !path.Contains("/auditlog") && // Avoid logging audit log operations
                !path.Contains("/seed") && // Skip seed operations
                !path.Contains("/health")) // Skip health checks
            {
                var (action, entityType) = DetermineActionAndEntity(method, path);

                var auditLog = new AuditLog
                {
                    Action = action,
                    EntityType = entityType,
                    EntityId = ExtractEntityIdFromPath(path),
                    UserId = userId,
                    UserEmail = userEmail,
                    Description = $"{action} {entityType} via {method} {path}",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = ip,
                    Details = $"Status: {statusCode}",
                    Severity = "Info"
                };

                try
                {
                    await auditLogService.AddAuditLogAsync(auditLog);
                }
                catch (Exception auditEx)
                {
                    Log.Warning(auditEx, "Failed to save audit log to database");
                }
            }
        }
        catch (Exception ex)
        {
            var statusCode = context.Response.StatusCode;
            Log.Error(ex, "Unhandled exception for {Method} {Path} {StatusCode} by {UserId}", method, path, statusCode, userId);

            // Log error to audit (only if feature is enabled)
            var auditEnabled = await featureManager.IsEnabledAsync("AuditLogging");
            if (auditEnabled)
            {
                try
                {
                    var (action, entityType) = DetermineActionAndEntity(method, path);
                    var auditLog = new AuditLog
                    {
                        Action = action,
                        EntityType = entityType,
                        EntityId = ExtractEntityIdFromPath(path),
                        UserId = userId,
                        UserEmail = userEmail,
                        Description = $"Failed {action} {entityType} via {method} {path}",
                        Timestamp = DateTime.UtcNow,
                        IpAddress = ip,
                        Details = $"Error: {ex.Message}",
                        Severity = "Error"
                    };
                    await auditLogService.AddAuditLogAsync(auditLog);
                }
                catch
                {
                    // Silently fail if audit logging fails during exception
                }
            }

            throw;
        }
    }

    private (string action, string entityType) DetermineActionAndEntity(string method, string path)
    {
        var action = method switch
        {
            "POST" => "CREATE",
            "PUT" => "UPDATE",
            "DELETE" => "DELETE",
            "GET" => "VIEW",
            _ => "UNKNOWN"
        };

        var entityType = "Unknown";
        if (path.Contains("/movies")) entityType = "Movie";
        else if (path.Contains("/screenings")) entityType = "Screening";
        else if (path.Contains("/bookings")) entityType = "Booking";
        else if (path.Contains("/users")) entityType = "User";
        else if (path.Contains("/theaterrooms")) entityType = "TheaterRoom";
        else if (path.Contains("/foodcombos")) entityType = "FoodCombo";
        else if (path.Contains("/foodorders")) entityType = "FoodOrder";
        else if (path.Contains("/cinemalocations")) entityType = "CinemaLocation";
        else if (path.Contains("/payments")) entityType = "Payment";
        else if (path.Contains("/tickets")) entityType = "Ticket";
        else if (path.Contains("/invoices")) entityType = "Invoice";

        return (action, entityType);
    }

    private string ExtractEntityIdFromPath(string path)
    {
        // Try to extract ID from common patterns like /api/entity/{id}
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length >= 3)
        {
            var lastSegment = segments[^1];
            // Check if last segment looks like a GUID or numeric ID
            if (Guid.TryParse(lastSegment, out _) || int.TryParse(lastSegment, out _))
            {
                return lastSegment;
            }
        }
        return "N/A";
    }
}
