using FluentResults;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Reviewers;

namespace web.Features.Browse.SkipArtPiece;

public class SkipArtPieceCommand(ApplicationDbContext dbContext)
{
    private const int SKIP_POINTS_COST = 5;

    public async Task<Result> ExecuteAsync(Guid currentUserId)
    {
        Reviewer reviewer = await dbContext.Reviewers.FirstAsync(r => r.UserId == currentUserId);
        if (reviewer.Points < SKIP_POINTS_COST)
        {
            return Result.Fail("You do not have enough points to skip an art piece.");  
        }

        reviewer.Points -= SKIP_POINTS_COST;

        ArtPieceServed? artPieceServed = await dbContext.ArtPiecesServed
            .FirstOrDefaultAsync(aps => aps.UserId == currentUserId);
        if (artPieceServed is not null)
        {
            dbContext.Remove(artPieceServed);
        }

        await dbContext.SaveChangesAsync();
        return Result.Ok();
    }
}