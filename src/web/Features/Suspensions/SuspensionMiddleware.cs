using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Features.Suspensions;

public class SuspensionMiddleware(RequestDelegate next)
{
        private readonly RequestDelegate _next = next;

        private static readonly PathString[] AllowedPaths =
        {
                "/Logout",
                "/Suspended",
        };

        public async Task InvokeAsync(
                HttpContext context,
                ApplicationDbContext dbContext)
        {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                        await _next(context);
                        return;
                }

                Guid userId = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                Suspension? activeSuspension = await dbContext.Suspensions
                        .Where(s => s.UserId == userId && s.ExpiryDate > DateTimeOffset.UtcNow)
                        .OrderByDescending(s => s.ExpiryDate)
                        .FirstOrDefaultAsync();

                context.Items["IsSuspended"] = activeSuspension is not null;

                PathString path = context.Request.Path;
                if (AllowedPaths.Any(path.StartsWithSegments))
                {
                        await _next(context);
                        return;
                }

                if (activeSuspension is not null)
                {
                        context.Response.Redirect("/Suspended");
                        return;
                }

                await _next(context);
        }
}
