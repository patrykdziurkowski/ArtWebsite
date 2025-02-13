using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Data;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Reviews;
using web.Features.Reviews.ReviewArtPiece;

namespace tests.Integration.Commands;

[Collection("Database collection")]
public class ReviewArtPieceCommandTests : IDisposable
{
        private readonly ReviewArtPieceCommand _command;
        private readonly UploadArtPieceCommand _uploadArtPiece;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;

        public ReviewArtPieceCommandTests(DatabaseTestContext databaseContext)
        {
                _scope = databaseContext.Services.CreateScope();
                _command = _scope.ServiceProvider.GetRequiredService<ReviewArtPieceCommand>();
                _uploadArtPiece = _scope.ServiceProvider.GetRequiredService<UploadArtPieceCommand>();
                _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();
                _dbContext.Database.BeginTransaction();
        }

        public void Dispose()
        {
                RemoveArtPieceImages();
                _dbContext.Database.RollbackTransaction();
                _scope.Dispose();
        }

        [Fact]
        public async Task Execute_CreatesReviewEntity_WhenSuccessful()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await _userManager.CreateAsync(user);
                _dbContext.Artists.Add(
                        new Artist(new ArtistId(), user.Id, "ArtistName",
                                "A profile summary for an artist."));
                await _dbContext.SaveChangesAsync();
                ArtPiece artPiece = await _uploadArtPiece.ExecuteAsync(
                        GetExampleFile(), "description", user.Id);

                await _command.ExecuteAsync("Review comment!", artPiece.Id, user.Id);

                _dbContext.Reviews.FirstOrDefault()
                        .Should().NotBeNull();
        }

        private FormFile GetExampleFile()
        {
                MemoryStream stream = new(Encoding.UTF8.GetBytes("Test file"));
                return new FormFile(stream, 0, stream.Length, "file", "myFile")
                {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpeg"
                };
        }

        private void RemoveArtPieceImages()
        {
                Directory.Delete("user-images", recursive: true);
        }
}
