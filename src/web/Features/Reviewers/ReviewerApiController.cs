using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web.Features.ArtPieces;
using web.Features.Reviewers.LikeArtPiece;
using web.Features.Reviewers.LoadLikes;

namespace web.Features.Reviewers;

[Authorize]
public class ReviewerApiController(LikeArtPieceCommand likeArtPieceCommand,
        LikesQuery likesQuery) : ControllerBase
{
        private const int LIKES_TO_LOAD = 10;

        [HttpGet("/api/reviewer/likes")]
        public async Task<IActionResult> LoadLikes([Range(0, int.MaxValue)] int offset = 0)
        {
                List<Like> likes = await likesQuery.ExecuteAsync(GetUserId(),
                        LIKES_TO_LOAD, offset);
                return Ok(likes);
        }

        [HttpPost("/api/artpieces/{artPieceId}/like")]
        public async Task<IActionResult> LikeArtPiece(Guid artPieceId)
        {
                Result<Like> result = await likeArtPieceCommand.ExecuteAsync(GetUserId(),
                        new ArtPieceId { Value = artPieceId });
                if (result.IsFailed)
                {
                        return Forbid();
                }

                return Created();
        }

        private Guid GetUserId()
        {
                string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("Could not find the user's id in class when expected.");
                return Guid.Parse(idClaim);
        }
}
