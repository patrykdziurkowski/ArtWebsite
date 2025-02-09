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

namespace tests.Integration.Commands;

[Collection("Database collection")]
public class UploadArtPieceCommandTests : IDisposable
{
        private readonly UploadArtPieceCommand _command;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;

        public UploadArtPieceCommandTests(DatabaseTestContext databaseContext)
        {
                _scope = databaseContext.Services.CreateScope();
                _command = _scope.ServiceProvider.GetRequiredService<UploadArtPieceCommand>();
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
        public async Task ExecuteAsync_SavesImageObject()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await _userManager.CreateAsync(user);
                ArtistId artistId = new();
                _dbContext.Artists.Add(
                        new Artist(artistId, user.Id, "ArtistName",
                                "A profile summary for an artist."));
                await _dbContext.SaveChangesAsync();


                ArtPiece artPiece = await _command.ExecuteAsync(
                        GetExampleFile(), "description", user.Id);

                _dbContext.ArtPieces.FirstOrDefault(a => a.Description == "description")
                        .Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_SavesImageAsAFile()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await _userManager.CreateAsync(user);
                ArtistId artistId = new();
                _dbContext.Artists.Add(
                        new Artist(artistId, user.Id, "ArtistName",
                                "A profile summary for an artist."));
                await _dbContext.SaveChangesAsync();

                ArtPiece artPiece = await _command.ExecuteAsync(
                        GetExampleFile(), "description", user.Id);

                string path = Path.Combine("user-images", "art-pieces",
                        $"{artistId}", $"{artPiece.Id}");
                File.Exists(path).Should().BeTrue();
        }

        private void RemoveArtPieceImages()
        {
                string path = Path.Combine("user-images", "art-pieces");
                Directory.Delete(path, recursive: true);
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

}
