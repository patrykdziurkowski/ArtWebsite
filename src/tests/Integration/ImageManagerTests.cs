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

                absoluteWebPath.Should().Be($"/user-images/art-pieces/{artistId}/{artPieceId}.png");
                File.Exists('.' + absoluteWebPath).Should().BeTrue();
        }

        [Fact]
        public async Task UpdateReviewerProfilePictureAsync_CreatesAFile()
        {
                ReviewerId reviewerId = new();

                string absoluteWebPath = await _imageManager.UpdateReviewerProfilePictureAsync(
                        GetExampleFile(), reviewerId);

                absoluteWebPath.Should().Be($"/user-images/profile-pictures/reviewers/{reviewerId}.png");
                File.Exists('.' + absoluteWebPath).Should().BeTrue();
        }

        [Fact]
        public async Task UpdateArtistProfilePictureAsync_CreatesAFile()
        {
                ArtistId artistId = new();

                string absoluteWebPath = await _imageManager.UpdateArtistProfilePictureAsync(
                        GetExampleFile(), artistId);

                absoluteWebPath.Should().Be($"/user-images/profile-pictures/artists/{artistId}.png");
                File.Exists('.' + absoluteWebPath).Should().BeTrue();
        }
}

