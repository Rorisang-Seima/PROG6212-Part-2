using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Api.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SessionGuardAttribute(params string[] roles) : Attribute, IAsyncActionFilter
{
    private readonly string[] _roles = roles ?? [];

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var session = context.HttpContext.Session;
        var userId = session.GetInt32("UserId");
        var role = session.GetString("Role");
        if (userId is null || string.IsNullOrWhiteSpace(role))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Please log in first." });
            return;
        }
        if (_roles.Length > 0 && !_roles.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new ObjectResult(new { message = "Your role is not permitted to perform this action." }) { StatusCode = 403 };
            return;
        }
        await next();
    }
}
