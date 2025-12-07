using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Leaderboard;
using web.Features.Leaderboard.Reviewer;
using web.Features.Missions;
using web.Features.Reviewers;
using web.Features.Reviews.ReviewArtPiece;
using web.Features.Tags;

namespace tests.Integration.Queries;

public class ReviewerLeaderboardQueryTests : DatabaseTest
{
        private readonly ReviewerLeaderboardQuery _query;
        private readonly UploadArtPieceCommand _uploadArtPieceCommand;
        private readonly ReviewArtPieceCommand _reviewArtPieceCommand;
        private readonly IMissionGenerator _mockMissionGenerator;

        public ReviewerLeaderboardQueryTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _query = Scope.ServiceProvider.GetRequiredService<ReviewerLeaderboardQuery>();
                ArtistRepository artistRepository = Scope.ServiceProvider.GetRequiredService<ArtistRepository>();
                IServiceScopeFactory serviceScopeFactory = Scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
                ImageTaggingQueue imageTaggingQueue = Scope.ServiceProvider.GetRequiredService<ImageTaggingQueue>();
                _mockMissionGenerator = Substitute.For<IMissionGenerator>();
                MissionManager missionManager = new(DbContext, _mockMissionGenerator);
                _uploadArtPieceCommand = new(DbContext, artistRepository, imageTaggingQueue, missionManager, serviceScopeFactory);
                _reviewArtPieceCommand = new(DbContext, missionManager);

                _mockMissionGenerator.GetMissions(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>(), 1)
                        .Returns([MissionType.BoostArt]);
        }

        [Fact]
        public async Task Execute_ReturnsTopReviewers()
        {
                await CreateUserWithArtistProfile();
                for (int i = 0; i < 25; i++)
                {
                        await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", DbContext.Users.First().Id);
                }
                List<ArtPiece> artPieces = await DbContext.ArtPieces.ToListAsync();

                const int TOTAL_REVIEWERS = 25;
                for (int i = 0; i < TOTAL_REVIEWERS; i++)
                {
                        ArtistId artistId = await CreateUserWithArtistProfile($"johnSmith{i}", $"artistName{i}");
                        Guid currentUserId = DbContext.Artists.Single(a => a.Id == artistId).UserId;

                        for (int j = 0; j < i; j++)
                        {
                                await _reviewArtPieceCommand.ExecuteAsync("Some comment.", rating: 3, artPieces[j].Id, currentUserId);
                        }
                }

                List<LeaderboardDto> topReviewers = await _query.ExecuteAsync(0, 10, TimeSpan.FromDays(365));

                topReviewers.Count.Should().Be(10);
                topReviewers.OrderByDescending(a => a.PointsInThatTimeSpan).Should().BeEquivalentTo(topReviewers);
                for (int i = 0; i < topReviewers.Count; i++)
                {
                        topReviewers[i].PointsInThatTimeSpan.Should().Be(10 * (TOTAL_REVIEWERS - i - 1));
                }
        }

        [Fact]
        public async Task Execute_ReturnsTopArtists_WithOffset()
        {
                await CreateUserWithArtistProfile();
                for (int i = 0; i < 25; i++)
                {
                        await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", DbContext.Users.First().Id);
                }
                List<ArtPiece> artPieces = await DbContext.ArtPieces.ToListAsync();

                const int TOTAL_REVIEWERS = 25;
                for (int i = 0; i < TOTAL_REVIEWERS; i++)
                {
                        ArtistId artistId = await CreateUserWithArtistProfile($"johnSmith{i}", $"artistName{i}");
                        Guid currentUserId = DbContext.Artists.Single(a => a.Id == artistId).UserId;

                        for (int j = 0; j < i; j++)
                        {
                                await _reviewArtPieceCommand.ExecuteAsync("Some comment.", rating: 3, artPieces[j].Id, currentUserId);
                        }
                }

                List<LeaderboardDto> topReviewers = await _query.ExecuteAsync(10, 5, TimeSpan.FromDays(365));

                topReviewers.Count.Should().Be(5);
                topReviewers.OrderByDescending(a => a.PointsInThatTimeSpan).Should().BeEquivalentTo(topReviewers);
                for (int i = 0; i < topReviewers.Count; i++)
                {
                        topReviewers[i].PointsInThatTimeSpan.Should().Be(10 * (TOTAL_REVIEWERS - 10 - i - 1));
                }
        }

        [Fact]
        public async Task Execute_ReturnsArtistsPoints_ForAGivenPeriodOfTime()
        {
                await CreateUserWithArtistProfile();
                for (int i = 0; i < 5; i++)
                {
                        await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", DbContext.Users.First().Id);
                }
                List<ArtPiece> artPieces = await DbContext.ArtPieces.ToListAsync();

                ArtistId artistId = await CreateUserWithArtistProfile("johnSmith1", "artistName1");
                Guid currentUserId = DbContext.Artists.Single(a => a.Id == artistId).UserId;

                for (int i = 0; i < 5; i++)
                {
                        await _reviewArtPieceCommand.ExecuteAsync("Some comment.", rating: 3, artPieces[i].Id, currentUserId);
                }

                var awards = await DbContext.ReviewerPointAwards.OrderByDescending(award => award.DateAwarded).ToListAsync();
                typeof(ReviewerPointAward).GetProperty(nameof(ReviewerPointAward.DateAwarded))!
                        .SetValue(awards[0], new DateTimeOffset(new DateTime(1994, 10, 25)));
                typeof(ReviewerPointAward).GetProperty(nameof(ReviewerPointAward.DateAwarded))!
                        .SetValue(awards[1], new DateTimeOffset(new DateTime(2010, 10, 25)));
                await DbContext.SaveChangesAsync();

                List<LeaderboardDto> topReviewers = await _query.ExecuteAsync(0, 10, TimeSpan.FromDays(7));

                topReviewers.Count.Should().Be(2);
                topReviewers.Should().Contain(r => r.PointsInThatTimeSpan == 30);
                topReviewers.Should().Contain(r => r.PointsInThatTimeSpan == 0);
        }

        [Fact]
        public async Task Execute_ReturnsAllTimeArtistsPoints_WhenNoTimeSpanSpecified()
        {
                await CreateUserWithArtistProfile();
                for (int i = 0; i < 5; i++)
                {
                        await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", DbContext.Users.First().Id);
                }
                List<ArtPiece> artPieces = await DbContext.ArtPieces.ToListAsync();

                ArtistId artistId = await CreateUserWithArtistProfile("johnSmith1", "artistName1");
                Guid currentUserId = DbContext.Artists.Single(a => a.Id == artistId).UserId;

                for (int i = 0; i < 5; i++)
                {
                        await _reviewArtPieceCommand.ExecuteAsync("Some comment.", rating: 3, artPieces[i].Id, currentUserId);
                }

                var awards = await DbContext.ReviewerPointAwards.OrderByDescending(award => award.DateAwarded).ToListAsync();
                typeof(ReviewerPointAward).GetProperty(nameof(ReviewerPointAward.DateAwarded))!
                        .SetValue(awards[0], new DateTimeOffset(new DateTime(1994, 10, 25)));
                typeof(ReviewerPointAward).GetProperty(nameof(ReviewerPointAward.DateAwarded))!
                        .SetValue(awards[1], new DateTimeOffset(new DateTime(2010, 10, 25)));
                await DbContext.SaveChangesAsync();

                List<LeaderboardDto> topReviewers = await _query.ExecuteAsync(0, 10);

                topReviewers.Count.Should().Be(2);
                topReviewers.Should().Contain(r => r.PointsInThatTimeSpan == 50);
                topReviewers.Should().Contain(r => r.PointsInThatTimeSpan == 0);
        }
}
