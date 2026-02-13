using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Features.ArtPieces;
using web.Features.Leaderboard.Artist;
using web.Features.Missions;

namespace web.Features.Artists.BoostArtPiece;

public class BoostArtPieceCommand(
        ArtistRepository artistRepository,
        ApplicationDbContext dbContext,
        MissionManager missionManager)
{
        public async Task<Result<BoostDto>> ExecuteAsync(
                Guid currentUserId,
                ArtPieceId artPieceId,
                DateTimeOffset? now = null)
        {
                now ??= DateTimeOffset.UtcNow;

                Artist currentArtist = await artistRepository.GetByUserIdAsync(currentUserId)
                        ?? throw new InvalidOperationException("No artist profile exists for the given user!");
                ArtPiece artPiece = await dbContext.ArtPieces
                        .SingleOrDefaultAsync(a => a.Id == artPieceId)
                                ?? throw new InvalidOperationException("Such art piece doesn't exist!");

                Result result = currentArtist.BoostArtPiece(artPieceId, artPiece.ArtistId);
                if (result.IsFailed)
                {
                        return result;
                }

                dbContext.ArtistPointAwards.Add(new ArtistPointAward()
                {
                        ArtistId = currentArtist.Id,
                        PointValue = 20,
                });

                await artistRepository.SaveChangesAsync(currentArtist);

                await missionManager.RecordProgressAsync(MissionType.BoostArt, currentUserId, now.Value);

                ArtPiece boostedArtPiece = await dbContext.ArtPieces
                        .SingleAsync(ap => ap.Id == currentArtist.ActiveBoost!.ArtPieceId);
                Boost boost = currentArtist.ActiveBoost!;
                BoostDto boostDto = new()
                {
                        Id = boost.Id,
                        Date = boost.Date,
                        ExpirationDate = boost.ExpirationDate,
                        ImagePath = boostedArtPiece.ImagePath,
                        ArtistId = boostedArtPiece.ArtistId,
                        ArtPieceId = boostedArtPiece.Id,

                };
                return Result.Ok(boostDto);
        }
}
