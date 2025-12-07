using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using tests.Integration.Fixtures;
using web.Features.ArtPieces;
using web.Features.Missions;
using web.Features.Reviewers;
using web.Features.Reviewers.LikeArtPiece;

namespace tests.Integration.Commands;

public class LikeArtPieceCommandTests : DatabaseTest
{
        private readonly LikeArtPieceCommand _command;

        private readonly IMissionGenerator _mockMissionGenerator;

        public LikeArtPieceCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                ReviewerRepository reviewerRepository = Scope.ServiceProvider.GetRequiredService<ReviewerRepository>();
                _mockMissionGenerator = Substitute.For<IMissionGenerator>();
                MissionManager missionManager = new(DbContext, _mockMissionGenerator);
                _command = new(reviewerRepository, missionManager);
        }

        [Fact]
        public async Task Execute_ShouldReturnFail_WhenLikedTooManyArtPieces()
        {
                List<ArtPieceId> artPieceIds = await CreateArtistUserWithArtPieces();
                Guid currentUserId = DbContext.Users.First().Id;
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

                await _command.ExecuteAsync(currentUserId, artPieceIds.First());

                DbContext.Likes.Should().HaveCount(1);
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
                        await _command.ExecuteAsync(currentUserId, artPieceIds[i]);
                }

                DbContext.Reviewers.Single(r => r.UserId == currentUserId).Points.Should().Be(25);
                DbContext.Likes.Should().HaveCount(numberOfLikesForMission);
        }
}
