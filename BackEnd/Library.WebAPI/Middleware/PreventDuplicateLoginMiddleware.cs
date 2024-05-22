using System.Security.Claims;
using System.Threading.Tasks;
using Library.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

public class PreventDuplicateLoginMiddleware
{
    private readonly RequestDelegate _next;

    public PreventDuplicateLoginMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null && context.Request.Path == "/login")
        {
            
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("You are already logged in. Please log out to login with another account.");
            return;
        }

        await _next(context);
    }
}
