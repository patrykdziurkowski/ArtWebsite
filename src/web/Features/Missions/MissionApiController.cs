using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.Features.Missions
{
        [Authorize]
        [ApiController]
        public class MissionApiController(TodaysMissionQuery todaysMissionQuery) : ControllerBase
        {
                [HttpGet("/api/mission")]
                public async Task<IActionResult> GetTodaysMission()
                {
                        TodaysMissionDto dto = await todaysMissionQuery.ExecuteAsync(GetUserId());
                        return Ok(dto);
                }

                private Guid GetUserId()
                {
                        string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                        return Guid.Parse(idClaim);
                }
        }
}
