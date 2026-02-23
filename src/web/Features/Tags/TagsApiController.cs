using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPieces;
using web.Features.Tags.AssignTag;
using web.Features.Tags.UnassignTag;

namespace web.Features.Tags;

[ApiController]
[Authorize]
public class TagsApiController(
        ArtPieceTagsQuery artPieceTagsQuery,
        AssignTagCommand assignTagCommand,
        UnassignTagCommand unassignTagCommand)
        : ControllerBase
{
        [HttpGet("/api/artpieces/{artPieceId}/tags")]
        public async Task<IActionResult> GetTags(Guid artPieceId)
        {
                List<Tag> tags = await artPieceTagsQuery.ExecuteAsync(new ArtPieceId() { Value = artPieceId });
                return Ok(tags.Select(t => t.Name).ToList());
        }

        [HttpDelete("/api/artpieces/{artPieceId}/tags/{tagName}")]
        public async Task<IActionResult> UnassignTagFromArtPiece(
                Guid artPieceId,
                [StringLength(16, MinimumLength = 2, ErrorMessage = "Tag name must be between 2 and 16 characters")]
                [RegularExpression(@"^[a-zA-Z0-9]+( [a-zA-Z0-9]+)*$", ErrorMessage = "Tag name can only contain alphanumeric characters or spaces")]
                string tagName)
        {
                await unassignTagCommand.ExecuteAsync(GetUserId(), new ArtPieceId() { Value = artPieceId }, tagName);
                return NoContent();
        }

        [HttpPost("/api/artpieces/{artPieceId}/tags")]
        public async Task<IActionResult> AssignTagToArtPiece(
                Guid artPieceId,
                [StringLength(16, MinimumLength = 2, ErrorMessage = "Tag name must be between 2 and 16 characters")]
                [RegularExpression(@"^[a-zA-Z0-9]+( [a-zA-Z0-9]+)*$", ErrorMessage = "Tag name can only contain alphanumeric characters or spaces")]
                string tagName)
        {
                await assignTagCommand.ExecuteAsync(GetUserId(), new ArtPieceId() { Value = artPieceId }, tagName);
                return NoContent();
        }
        

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
