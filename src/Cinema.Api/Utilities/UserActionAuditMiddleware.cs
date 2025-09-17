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

        Log.Information("AUDIT {Method} {Path} by {UserId} from {IP}", method, path, userId, ip);

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception for {Method} {Path} by {UserId}", method, path, userId);
            throw;
        }
    }
}
