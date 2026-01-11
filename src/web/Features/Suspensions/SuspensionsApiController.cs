using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.Features.Suspensions;

[ApiController]
[Authorize(Roles = Constants.ADMIN_ROLE)]
public class SuspensionsApiController(SuspendUserCommand suspendUserCommand) : ControllerBase
{
        [HttpPost("/api/users/{userToSuspendId}/suspensions")]
        public async Task<IActionResult> SuspendUser(Guid userToSuspendId, SuspendModel model)
        {
                Suspension suspension = await suspendUserCommand.ExecuteAsync(
                        GetUserId(),
                        userToSuspendId,
                        TimeSpan.FromMinutes(model.DurationMinutes),
                        model.Reason);
                return Ok(suspension);
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
