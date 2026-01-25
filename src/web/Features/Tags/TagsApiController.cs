using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPieces;
using web.Features.Search;

namespace web.Features.Tags;

[ApiController]
[Authorize]
public class TagsApiController(
        ArtPieceTagsQuery artPieceTagsQuery,
        SearchByNameQuery tagsByNameQuery)
        : ControllerBase
{
        [HttpGet("/api/artpieces/{artPieceId}/tags")]
        public async Task<IActionResult> GetTags(Guid artPieceId)
        {
                List<Tag> tags = await artPieceTagsQuery.ExecuteAsync(new ArtPieceId() { Value = artPieceId });
                return Ok(tags.Select(t => t.Name).ToList());
        }

        [HttpGet("/api/tags")]
        public async Task<IActionResult> GetTagsByName([FromQuery] string name)
        {
                if (string.IsNullOrWhiteSpace(name))
                {
                        return Ok(Array.Empty<string>());
                }


                SearchDto result = await tagsByNameQuery.ExecuteAsync(name);
                return Ok(result.Tags);
        }
}
