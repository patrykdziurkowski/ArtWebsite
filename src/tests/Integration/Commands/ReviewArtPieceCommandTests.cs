using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Missions;
using web.Features.Reviewers;
using web.Features.Reviews.ReviewArtPiece;

namespace tests.Integration.Commands;

public class ReviewArtPieceCommandTests : DatabaseTest
{
        private readonly ReviewArtPieceCommand _command;
        private readonly UploadArtPieceCommand _uploadArtPiece;
        private readonly IMissionGenerator _mockMissionGenerator;

        public ReviewArtPieceCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _uploadArtPiece = Scope.ServiceProvider.GetRequiredService<UploadArtPieceCommand>();
                _mockMissionGenerator = Substitute.For<IMissionGenerator>();
                MissionManager missionManager = new(DbContext, _mockMissionGenerator);
                _command = new(DbContext, missionManager);
        }

        [Fact]
        public async Task Execute_CreatesReviewEntity_WhenSuccessful()
        {
                _mockMissionGenerator.GetMissions(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>(), 1)
                        .Returns([MissionType.BoostArt]);
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                DbContext.Reviewers.Add(new Reviewer()
                {
                        Name = "SomeUser123",
                        UserId = user.Id,
                });
                DbContext.Artists.Add(new Artist()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                });
                await DbContext.SaveChangesAsync();
                ArtPiece artPiece = await _uploadArtPiece.ExecuteAsync(
                        GetExampleFile(), "description", user.Id);

                await _command.ExecuteAsync("Review comment!", 5, artPiece.Id, user.Id);

                DbContext.Reviews.FirstOrDefault().Should().NotBeNull();
                DbContext.Reviewers.First().Points.Should().Be(10);
                DbContext.ReviewerPointAwards.Single().PointValue.Should().Be(10);
        }

        [Fact]
        public async Task Execute_CompletesMission_WhenAReviewMissionIsPresent()
        {
                _mockMissionGenerator.GetMissions(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>(), 1)
                        .Returns([MissionType.ReviewArt]);
                const int POINTS_PER_REVIEW = 10;
                const int POINTS_PER_QUEST = 25;
                int countOfReviewsToPerform = MissionType.ReviewArt.GetMaxProgressCount();

                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                DbContext.Reviewers.Add(new Reviewer()
                {
                        Name = "SomeUser123",
                        UserId = user.Id,
                });
                DbContext.Artists.Add(new Artist()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                });
                await DbContext.SaveChangesAsync();


                for (int i = 0; i < countOfReviewsToPerform; i++)
                {
                        ArtPiece artPiece = await _uploadArtPiece.ExecuteAsync(
                                GetExampleFile(), "description", user.Id);
                        await _command.ExecuteAsync("Review comment!", 5, artPiece.Id, user.Id);
                }


                DbContext.Reviewers.Single().Points.Should().Be(countOfReviewsToPerform * POINTS_PER_REVIEW + POINTS_PER_QUEST);
        }

}
