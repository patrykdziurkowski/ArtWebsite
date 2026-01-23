using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.Images;
using web.Features.Reviewers;

namespace tests.Integration;

public class ImageManagerTests : DatabaseTest
{
        private readonly ImageManager _imageManager;

        public ImageManagerTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _imageManager = Scope.ServiceProvider.GetRequiredService<ImageManager>();
        }

        [Fact]
        public async Task SaveArtPieceImageAsync_CreatesAFile()
        {
                ArtistId artistId = new();
                ArtPieceId artPieceId = new();

                string absoluteWebPath = await _imageManager.SaveArtPieceImageAsync(
                        GetExampleFile(), artistId, artPieceId);

                absoluteWebPath.Should().StartWith("/user-images/");
                absoluteWebPath.Should().Contain(artistId.ToString());
                absoluteWebPath.Should().Contain(artPieceId.ToString());
                Path.GetExtension(absoluteWebPath).Should().NotBeNullOrEmpty();
                File.Exists('.' + absoluteWebPath).Should().BeTrue();
        }

        [Fact]
        public async Task UpdateReviewerProfilePictureAsync_CreatesAFile()
        {
                ReviewerId reviewerId = new();

                string absoluteWebPath = await _imageManager.UpdateReviewerProfilePictureAsync(
                        GetExampleFile(), reviewerId);

                absoluteWebPath.Should().StartWith("/user-images/");
                absoluteWebPath.Should().Contain(reviewerId.ToString());
                Path.GetExtension(absoluteWebPath).Should().NotBeNullOrEmpty();
                File.Exists('.' + absoluteWebPath).Should().BeTrue();
        }
}
