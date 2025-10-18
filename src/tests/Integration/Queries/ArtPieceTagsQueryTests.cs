using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Tags;

namespace tests.Integration.Queries;

public class ArtPieceTagsQueryTests : DatabaseTest
{
        private readonly ArtPieceTagsQuery _query;
        private readonly UploadArtPieceCommand _uploadArtPiece;
        private readonly ImageTaggingQueue _imageTaggingQueue;

        public ArtPieceTagsQueryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _query = Scope.ServiceProvider.GetRequiredService<ArtPieceTagsQuery>();

                _imageTaggingQueue = Scope.ServiceProvider.GetRequiredService<ImageTaggingQueue>();
                _uploadArtPiece = new(
                        DbContext,
                        Scope.ServiceProvider.GetRequiredService<ArtistRepository>(),
                        _imageTaggingQueue,
                        Scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmpty_WhenArtPieceDoesntExist()
        {
                List<Tag> tags = await _query.ExecuteAsync(new ArtPieceId());

                tags.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnTags_WhenArtPieceExists()
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
                ArtPiece artPiece = await _uploadArtPiece.ExecuteAsync(
                        GetExampleFile(), "description", user.Id);
                await WaitWhileNoTagsInDatabaseAsync();

                List<Tag> tags = await _query.ExecuteAsync(artPiece.Id);


                tags.Should().NotBeEmpty();
        }
}
