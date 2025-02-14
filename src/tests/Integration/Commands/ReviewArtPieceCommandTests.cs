using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Reviews.ReviewArtPiece;

namespace tests.Integration.Commands;

public class ReviewArtPieceCommandTests : DatabaseBase
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
                DbContext.Artists.Add(
                        new Artist(new ArtistId(), user.Id, "ArtistName",
                                "A profile summary for an artist."));
                await DbContext.SaveChangesAsync();
                ArtPiece artPiece = await _uploadArtPiece.ExecuteAsync(
                        GetExampleFile(), "description", user.Id);

                await _command.ExecuteAsync("Review comment!", artPiece.Id, user.Id);

                DbContext.Reviews.FirstOrDefault()
                        .Should().NotBeNull();
        }

}
