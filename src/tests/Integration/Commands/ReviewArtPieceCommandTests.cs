using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Reviewers;
using web.Features.Reviews.ReviewArtPiece;

namespace tests.Integration.Commands;

public class ReviewArtPieceCommandTests : DatabaseTest
{
        private readonly ReviewArtPieceCommand _command;
        private readonly UploadArtPieceCommand _uploadArtPiece;

        public ReviewArtPieceCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = Scope.ServiceProvider.GetRequiredService<ReviewArtPieceCommand>();
                _uploadArtPiece = Scope.ServiceProvider.GetRequiredService<UploadArtPieceCommand>();
        }

        [Fact]
        public async Task Execute_CreatesReviewEntity_WhenSuccessful()
        {
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

                DbContext.Reviews.FirstOrDefault()
                        .Should().NotBeNull();
                DbContext.Reviewers.First().Points.Should().Be(10);
                DbContext.ReviewerPointAwards.Single().PointValue.Should().Be(10);
        }

}
