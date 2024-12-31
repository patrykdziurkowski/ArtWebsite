using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.integration.fixtures;
using web.data;
using web.features.artist;
using web.Features.ArtPiece;
using web.Features.ArtPiece.UploadArtPiece;

namespace tests.Integration.Commands;

[Collection("Database collection")]
public class UploadArtPieceCommandTests : IDisposable
{
        private readonly UploadArtPieceCommand _command;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;
        private readonly string _rootDirectory;

        public UploadArtPieceCommandTests(DatabaseTestContext databaseContext)
        {
                _scope = databaseContext.Services.CreateScope();
                _command = _scope.ServiceProvider.GetRequiredService<UploadArtPieceCommand>();
                _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                IWebHostEnvironment webHostEnvironment = databaseContext
                        .Services.GetRequiredService<IWebHostEnvironment>();
                _rootDirectory = webHostEnvironment.ContentRootPath;

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
                IdentityUser user = new("johnSmith");
                await _userManager.CreateAsync(user);
                ArtistId artistId = new();
                _dbContext.Artists.Add(
                        new Artist(artistId, user.Id, "ArtistName",
                                "A profile summary for an artist."));
                await _dbContext.SaveChangesAsync();


                ArtPiece artPiece = await _command.ExecuteAsync(
                        GetExampleFile(), "description", artistId);

                _dbContext.ArtPieces.FirstOrDefault(a => a.Description == "description")
                        .Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_SavesImageAsAFile()
        {
                IdentityUser user = new("johnSmith");
                await _userManager.CreateAsync(user);
                ArtistId artistId = new();
                _dbContext.Artists.Add(
                        new Artist(artistId, user.Id, "ArtistName",
                                "A profile summary for an artist."));
                await _dbContext.SaveChangesAsync();

                ArtPiece artPiece = await _command.ExecuteAsync(
                        GetExampleFile(), "description", artistId);

                string path = Path.Combine(_rootDirectory, "wwwroot",
                        "images", "art-pieces", $"{artistId}", $"{artPiece.Id}");
                File.Exists(path).Should().BeTrue();
        }

        private void RemoveArtPieceImages()
        {
                string path = Path.Combine(_rootDirectory, "wwwroot",
                        "images", "art-pieces");
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
