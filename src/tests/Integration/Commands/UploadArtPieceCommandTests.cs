using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Tags;

namespace tests.Integration.Commands;

public class UploadArtPieceCommandTests : DatabaseBase
{
        private readonly UploadArtPieceCommand _command;

        public UploadArtPieceCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = new(
                        DbContext,
                        Scope.ServiceProvider.GetRequiredService<ArtistRepository>(),
                        Scope.ServiceProvider.GetRequiredService<ImageTaggingQueue>(),
                        Scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>());
        }

        [Fact]
        public async Task ExecuteAsync_SavesImageObject()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                DbContext.Artists.Add(new Artist
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                });

                await DbContext.SaveChangesAsync();


                ArtPiece artPiece = await _command.ExecuteAsync(
                        GetExampleFile(), "description", user.Id);

                DbContext.ArtPieces.FirstOrDefault(a => a.Description == "description")
                        .Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_SavesImageAsAFile()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                ArtistId artistId = new();
                DbContext.Artists.Add(new Artist
                {
                        Id = artistId,
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                });
                await DbContext.SaveChangesAsync();

                ArtPiece artPiece = await _command.ExecuteAsync(
                        GetExampleFile(), "description", user.Id);

                string path = Path.Combine("user-images", "art-pieces",
                        $"{artistId}", $"{artPiece.Id}.png");
                File.Exists(path).Should().BeTrue();
        }

}
