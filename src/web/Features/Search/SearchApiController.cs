using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.Features.Search;

[ApiController]
[Authorize]
public class SearchApiController(SearchByNameQuery searchByNameQuery) : ControllerBase
{
        [HttpGet("/api/search")]
        public async Task<IActionResult> GetTagsByName([FromQuery] string name)
        {
                if (string.IsNullOrWhiteSpace(name))
                {
                        return Ok(Array.Empty<string>());
                }


                SearchDto result = await searchByNameQuery.ExecuteAsync(name);
                return Ok(result);
        }
}
