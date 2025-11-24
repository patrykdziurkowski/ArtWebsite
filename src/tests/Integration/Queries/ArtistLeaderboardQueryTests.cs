using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.PointAwards;
using web.Features.PointAwards.Artist;

namespace tests.Integration.Queries;

public class ArtistLeaderboardQueryTests : DatabaseTest
{
        private readonly ArtistLeaderboardQuery _query;
        private readonly UploadArtPieceCommand _uploadArtPieceCommand;

        public ArtistLeaderboardQueryTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _query = Scope.ServiceProvider.GetRequiredService<ArtistLeaderboardQuery>();
                _uploadArtPieceCommand = Scope.ServiceProvider.GetRequiredService<UploadArtPieceCommand>();
        }

        [Fact]
        public async Task Execute_ReturnsTopArtists()
        {
                const int TOTAL_ARTISTS = 25;
                for (int i = 0; i < TOTAL_ARTISTS; i++)
                {
                        ArtistId artistId = await CreateUserWithArtistProfile($"johnSmith{i}", $"artistName{i}");
                        for (int j = 0; j < i; j++)
                        {
                                await _uploadArtPieceCommand.ExecuteAsync(
                                        GetExampleFile(),
                                        "description",
                                        DbContext.Artists.Single(a => a.Id == artistId).UserId);
                        }
                }

                List<LeaderboardDto> topArtists = await _query.ExecuteAsync(0, 10, TimeSpan.FromDays(365));

                topArtists.Count.Should().Be(10);
                topArtists.OrderByDescending(a => a.PointsInThatTimeSpan).Should().BeEquivalentTo(topArtists);
                for (int i = 0; i < topArtists.Count; i++)
                {
                        topArtists[i].PointsInThatTimeSpan.Should().Be(10 * (TOTAL_ARTISTS - i - 1));
                }
        }

        [Fact]
        public async Task Execute_ReturnsTopArtists_WithOffset()
        {
                const int TOTAL_ARTISTS = 25;
                for (int i = 0; i < TOTAL_ARTISTS; i++)
                {
                        ArtistId artistId = await CreateUserWithArtistProfile($"johnSmith{i}", $"artistName{i}");
                        for (int j = 0; j < i; j++)
                        {
                                await _uploadArtPieceCommand.ExecuteAsync(
                                        GetExampleFile(),
                                        "description",
                                        DbContext.Artists.Single(a => a.Id == artistId).UserId);
                        }
                }

                List<LeaderboardDto> topArtists = await _query.ExecuteAsync(10, 5, TimeSpan.FromDays(365));

                topArtists.Count.Should().Be(5);
                topArtists.OrderByDescending(a => a.PointsInThatTimeSpan).Should().BeEquivalentTo(topArtists);
                for (int i = 0; i < topArtists.Count; i++)
                {
                        topArtists[i].PointsInThatTimeSpan.Should().Be(10 * (TOTAL_ARTISTS - 10 - i - 1));
                }
        }

        [Fact]
        public async Task Execute_ReturnsArtistsPoints_ForAGivenPeriodOfTime()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                for (int j = 0; j < 5; j++)
                {
                        await _uploadArtPieceCommand.ExecuteAsync(
                                GetExampleFile(),
                                "description",
                                DbContext.Artists.Single(a => a.Id == artistId).UserId);
                }

                var awards = await DbContext.ArtistPointAwards.OrderByDescending(award => award.DateAwarded).ToListAsync();
                typeof(ArtistPointAward).GetProperty(nameof(ArtistPointAward.DateAwarded))!
                        .SetValue(awards[0], new DateTimeOffset(new DateTime(1994, 10, 25)));
                typeof(ArtistPointAward).GetProperty(nameof(ArtistPointAward.DateAwarded))!
                        .SetValue(awards[1], new DateTimeOffset(new DateTime(2010, 10, 25)));
                await DbContext.SaveChangesAsync();

                List<LeaderboardDto> topArtists = await _query.ExecuteAsync(0, 10, TimeSpan.FromDays(7));

                topArtists.Count.Should().Be(1);
                topArtists.Single().PointsInThatTimeSpan.Should().Be(30);
        }

        [Fact]
        public async Task Execute_ReturnsAllTimeArtistsPoints_WhenNoTimeSpecified()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                for (int j = 0; j < 5; j++)
                {
                        await _uploadArtPieceCommand.ExecuteAsync(
                                GetExampleFile(),
                                "description",
                                DbContext.Artists.Single(a => a.Id == artistId).UserId);
                }

                var awards = await DbContext.ArtistPointAwards.OrderByDescending(award => award.DateAwarded).ToListAsync();
                for (int i = 0; i < 5; i++)
                {
                        typeof(ArtistPointAward).GetProperty(nameof(ArtistPointAward.DateAwarded))!
                                .SetValue(awards[i], new DateTimeOffset(new DateTime(1970, 10, 25)));
                }
                await DbContext.SaveChangesAsync();

                List<LeaderboardDto> topArtists = await _query.ExecuteAsync(0, 10);

                topArtists.Count.Should().Be(1);
                topArtists.Single().PointsInThatTimeSpan.Should().Be(50);
        }
}
