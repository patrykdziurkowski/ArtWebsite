using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Browse;
using web.Features.Missions;
using web.Features.Reviewers;
using web.Features.Reviews.ReviewArtPiece;

namespace tests.Integration.Commands;

public class ReviewArtPieceCommandTests : DatabaseTest
{
        private readonly ReviewArtPieceCommand _command;
        private readonly UploadArtPieceCommand _uploadArtPiece;
        private readonly RegisterArtPieceServedCommand _registerArtPieceServedCommand;
        private readonly IMissionGenerator _mockMissionGenerator;

        public ReviewArtPieceCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _uploadArtPiece = Scope.ServiceProvider.GetRequiredService<UploadArtPieceCommand>();
                _mockMissionGenerator = Substitute.For<IMissionGenerator>();
                MissionManager missionManager = new(DbContext, _mockMissionGenerator);
                _command = new(DbContext, missionManager);
                _registerArtPieceServedCommand = Scope.ServiceProvider.GetRequiredService<RegisterArtPieceServedCommand>();
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
                await _registerArtPieceServedCommand.ExecuteAsync(user.Id, artPiece.Id);

                await _command.ExecuteAsync(
                        "Review comment!",
                        5,
                        artPiece.Id,
                        user.Id,
                        reviewCooldown: TimeSpan.Zero);

                DbContext.Reviews.FirstOrDefault().Should().NotBeNull();
                DbContext.Reviewers.First().Points.Should().Be(10);
                DbContext.ReviewerPointAwards.Single().PointValue.Should().Be(10);
        }

        [Fact]
        public async Task Execute_Throws_WhenReviewingTheSameArtPieceMoreThanOnce()
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
                await _registerArtPieceServedCommand.ExecuteAsync(user.Id, artPiece.Id);
                await _command.ExecuteAsync(
                        "Review comment!",
                        5,
                        artPiece.Id,
                        user.Id,
                        reviewCooldown: TimeSpan.Zero);

                Func<Task> executingSecondReview = async () => await _command
                        .ExecuteAsync(
                                "Review comment!",
                                5,
                                artPiece.Id,
                                user.Id,
                                reviewCooldown: TimeSpan.Zero);

                await executingSecondReview.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Execute_Throws_WhenReviewingAnArtPieceTooQuick()
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
                await _registerArtPieceServedCommand.ExecuteAsync(user.Id, artPiece.Id);

                Func<Task> reviewingTooFast = async () => await _command
                        .ExecuteAsync(
                                "Review comment!",
                                5,
                                artPiece.Id,
                                user.Id,
                                reviewCooldown: TimeSpan.FromSeconds(10));

                await reviewingTooFast.Should().ThrowAsync<InvalidOperationException>();
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
                        await _registerArtPieceServedCommand.ExecuteAsync(user.Id, artPiece.Id);
                        await _command.ExecuteAsync(
                                "Review comment!",
                                5,
                                artPiece.Id,
                                user.Id,
                                reviewCooldown: TimeSpan.Zero);
                }


                DbContext.Reviewers.Single().Points.Should().Be(countOfReviewsToPerform * POINTS_PER_REVIEW + POINTS_PER_QUEST);
        }

}
