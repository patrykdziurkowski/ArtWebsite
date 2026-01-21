using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Images;

namespace tests.Integration;

public class ImageManagerTests : DatabaseTest
{
        private readonly ImageManager _imageManager;

        public ImageManagerTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                _imageManager = Scope.ServiceProvider.GetRequiredService<ImageManager>();
        }

        [Theory]
        [InlineData("some-folder")]
        [InlineData("/some-folder")]
        [InlineData("some-folder/")]
        [InlineData("/some-folder/")]
        [InlineData("some-folder/other")]
        [InlineData("/some-folder/other")]
        [InlineData("some-folder/other/")]
        [InlineData("/some-folder/other/")]
        public async Task SaveOrUpdateImageAsync_CreatesAFile(string folder)
        {
                string absoluteWebPath = await _imageManager.SaveOrUpdateImageAsync(
                        GetExampleFile(), folder, "someFile");

                absoluteWebPath.StartsWith('/').Should().BeTrue();
                Path.GetExtension(absoluteWebPath).Should().NotBeNullOrEmpty();
                absoluteWebPath.Should().Be(
                        Path.Combine("/user-images/", folder.TrimStart(Path.DirectorySeparatorChar), "someFile.png"));
                File.Exists('.' + absoluteWebPath).Should().BeTrue();
        }
}
