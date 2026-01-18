using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using tests.Integration.Fixtures;
using web.Features.ArtPieces;
using web.Features.Browse;
using web.Features.Missions;
using web.Features.Reviewers;
using web.Features.Reviewers.LikeArtPiece;
using web.Features.Reviews.ReviewArtPiece;

namespace tests.Integration.Commands;

public class LikeArtPieceCommandTests : DatabaseTest
{
        private readonly LikeArtPieceCommand _command;
        private readonly ReviewArtPieceCommand _reviewArtPiece;
        private readonly RegisterArtPieceServedCommand _registerArtPieceServedCommand;
        private readonly IMissionGenerator _mockMissionGenerator;

        public LikeArtPieceCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                ReviewerRepository reviewerRepository = Scope.ServiceProvider.GetRequiredService<ReviewerRepository>();
                _registerArtPieceServedCommand = Scope.ServiceProvider.GetRequiredService<RegisterArtPieceServedCommand>();
                _mockMissionGenerator = Substitute.For<IMissionGenerator>();
                MissionManager missionManager = new(DbContext, _mockMissionGenerator);
                _command = new(reviewerRepository, DbContext, missionManager);
                _reviewArtPiece = new(DbContext, missionManager);
        }

        [Fact]
        public async Task Execute_ShouldThrow_WhenGivenArtPieceNotReviewed()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;

                Func<Task> executingLikeCommand = async () => await _command.ExecuteAsync(currentUserId, artPieceIds.First());

                await executingLikeCommand.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Execute_ShouldThrow_WhenLikingTheSameArtPieceMoreThanOnce()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;
                await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPieceIds.First());
                await _reviewArtPiece.ExecuteAsync(
                        "some comment some comment some comment some comment some comment some comment some comment some comment",
                        3,
                        artPieceIds.First(),
                        currentUserId,
                        reviewCooldown: TimeSpan.Zero);
                await _command.ExecuteAsync(currentUserId, artPieceIds.First());

                Func<Task> executingLikeCommand = async () => await _command.ExecuteAsync(currentUserId, artPieceIds.First());

                await executingLikeCommand.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Execute_ShouldReturnFail_WhenLikedTooManyArtPieces()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;
                for (int i = 0; i < 6; i++)
                {
                        await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPieceIds[i]);
                        await _reviewArtPiece.ExecuteAsync(
                                "some comment some comment some comment some comment some comment some comment some comment some comment",
                                3,
                                artPieceIds[i],
                                currentUserId,
                                reviewCooldown: TimeSpan.Zero);
                }

                List<Result<Like>> results = [];
                for (int i = 0; i < 6; ++i)
                {
                        Result<Like> result = await _command.ExecuteAsync(currentUserId, artPieceIds[i]);
                        results.Add(result);
                }

                DbContext.Likes.Should().HaveCount(5);
                for (int i = 0; i < 5; ++i)
                {
                        results[i].IsSuccess.Should().BeTrue();
                }
                results[5].IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Execute_ShouldLikeArtPiece_WhenSuccess()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;
                await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPieceIds.First());
                await _reviewArtPiece.ExecuteAsync(
                        "some comment some comment some comment some comment some comment some comment some comment some comment",
                        3,
                        artPieceIds.First(),
                        currentUserId,
                        reviewCooldown: TimeSpan.Zero);

                await _command.ExecuteAsync(currentUserId, artPieceIds.First());

                DbContext.Likes.Should().HaveCount(1);
                DbContext.ArtPieces.Single(ap => ap.Id == artPieceIds.First()).LikeCount.Should().Be(1);
        }

        [Fact]
        public async Task Execute_ShouldAddExtraPoints_WhenMissionCompleted()
        {
                _mockMissionGenerator.GetMissions(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>(), 1)
                        .Returns([MissionType.LikeArt]);
                int numberOfLikesForMission = MissionType.LikeArt.GetMaxProgressCount();
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;
                for (int i = 0; i < numberOfLikesForMission; i++)
                {
                        await _registerArtPieceServedCommand.ExecuteAsync(currentUserId, artPieceIds[i]);
                        await _reviewArtPiece.ExecuteAsync(
                                "some comment some comment some comment some comment some comment some comment some comment some comment",
                                3,
                                artPieceIds[i],
                                currentUserId,
                                reviewCooldown: TimeSpan.Zero);
                }

                for (int i = 0; i < numberOfLikesForMission; i++)
                {
                        await _command.ExecuteAsync(currentUserId, artPieceIds[i]);
                }

                DbContext.Reviewers.Single(r => r.UserId == currentUserId).Points.Should().Be(25 + numberOfLikesForMission * 10);
                DbContext.Likes.Should().HaveCount(numberOfLikesForMission);
        }
}
