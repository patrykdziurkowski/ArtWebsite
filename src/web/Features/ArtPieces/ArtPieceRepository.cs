using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.Reviewers;
using web.Features.Tags;

namespace web.Features.ArtPieces;

public class ArtPieceRepository(ApplicationDbContext dbContext)
{
        public async Task<ArtPiece?> GetByAlgorithmAsync(ReviewerId reviewerId, string? tagName = null)
        {
                List<ArtPieceId> reviewedArtPieceIds = await dbContext.Reviews
                        .Where(r => r.ReviewerId == reviewerId)
                        .Select(r => r.ArtPieceId)
                        .ToListAsync();

                TagId? tagToFilterById = null;
                if (!string.IsNullOrWhiteSpace(tagName))
                {
                        tagToFilterById = await dbContext.Tags
                                .Where(t => t.Name == tagName)
                                .Select(t => t.Id)
                                .FirstOrDefaultAsync();

                        if (tagToFilterById is null)
                        {
                                // no such tag exists
                                return null;
                        }
                }

                var query =
                        from ap in dbContext.ArtPieces
                        join artist in dbContext.Artists on ap.ArtistId equals artist.Id
                        where !reviewedArtPieceIds.Contains(ap.Id)
                        select new { ap, artist };

                if (tagToFilterById is not null)
                {
                        query = query.Where(artPieceAndItsArtist =>
                                dbContext.ArtPieceTags.Any(at =>
                                        at.ArtPieceId == artPieceAndItsArtist.ap.Id &&
                                        at.TagId == tagToFilterById));
                }

                return await query
                        .OrderByDescending(artPieceAndItsArtist =>
                                // Random factor: (0: +infinity) with rapidly decreasing likelihood
                                // of an increasingly bigger value.
                                2 * Math.Tan(dbContext.Random() - 1 + Math.PI / 2)
                                * (
                                        // Artist's points factor: [0: 1) with diminishing returns for higher and
                                        // higher point values. This uses a softsign function.
                                        artPieceAndItsArtist.artist.Points / 1000.0
                                        / (Math.Abs(artPieceAndItsArtist.artist.Points / 1000.0) + 1)
                                        // Another factor for other features of a given art piece.
                                        * (
                                                // Art piece average rating term: [0, 20, 40, 60, 80, 100]
                                                artPieceAndItsArtist.ap.AverageRating * 20.0
                                                // Art piece upload date term: (0: 100] with diminishing returns
                                                // for older art pieces. This uses a softsign function.
                                                + 100 + 100.0 * (
                                                        EF.Functions.DateDiffDay(
                                                                DateTimeOffset.UtcNow,
                                                                artPieceAndItsArtist.ap.UploadDate
                                                        ) / 10.0
                                                        / (
                                                                Math.Abs(
                                                                        EF.Functions.DateDiffDay(
                                                                                DateTimeOffset.UtcNow,
                                                                                artPieceAndItsArtist.ap.UploadDate
                                                                        ) / 10.0
                                                                ) + 1
                                                        )
                                                )
                                                // Art piece boosted: 150 extra points if boosted, 0 otherwise
                                                + (
                                                        (artPieceAndItsArtist.artist.ActiveBoost != null
                                                        && artPieceAndItsArtist.artist.ActiveBoost.ArtPieceId
                                                                == artPieceAndItsArtist.ap.Id)
                                                        ? 150
                                                        : 0
                                                )
                                        )
                                        +
                                        // Base term: used to ensure somewhat even starting point for later
                                        // multiplication. Change this to adjust how many popular vs unpopular
                                        // art pieces appear.
                                        1
                                )
                        )
                        .Select(artPieceAndItsArtist => artPieceAndItsArtist.ap)
                        .FirstOrDefaultAsync();
        }
}
