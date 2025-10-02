using Serilog;

namespace Cinema.Api.Utilities;

public class UserActionAuditMiddleware
{
    private readonly RequestDelegate _next;

    public UserActionAuditMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        var userId = context.User?.FindFirst("user_id")?.Value
                     ?? context.User?.FindFirst("uid")?.Value
                     ?? "anonymous";

        var path = context.Request.Path.Value;
        var method = context.Request.Method;
        var ip = context.Connection.RemoteIpAddress?.ToString();

        try
        {
            await _next(context);
            var statusCode = context.Response.StatusCode;
            Log.Information("AUDIT {Method} {Path} {StatusCode} by {UserId} from {IP}", method, path, statusCode, userId, ip);
        }
        catch (Exception ex)
        {
            var statusCode = context.Response.StatusCode;
            Log.Error(ex, "Unhandled exception for {Method} {Path} {StatusCode} by {UserId}", method, path, statusCode, userId);
            throw;
        }
    }
}
