using FluentAssertions;
using tests.Integration.Fixtures;
using web.Features.Tags.ImageRecognition;

namespace tests.Integration;

public class ImageTaggingQueueTests : AiContainerTests
{
        private readonly string _imagePath;
        private readonly ImageTaggingQueue _imageTaggingQueue = new(new ImageTagger()
        {
                Url = "http://localhost:8081/tag"
        });

        public ImageTaggingQueueTests()
        {
                _imagePath = Path.GetFullPath(Path.Combine(
                        typeof(ImageTaggingQueue).Assembly.Location,
                        "..",
                        "..",
                        "..",
                        "..",
                        "resources",
                        "exampleImage.png"));
                File.Exists(_imagePath).Should().BeTrue();
        }

        [Fact]
        public async Task Add_ShouldThrow_WhenSameImageQueuedTwice()
        {
                _ = _imageTaggingQueue.Add(_imagePath, (tags) => { });
                Func<Task> queuingImage = async () => await _imageTaggingQueue.Add(_imagePath, (tags) => { });

                await queuingImage.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Add_ShouldWork()
        {
                await _imageTaggingQueue.Add(_imagePath, (tags) =>
                {
                        _imageTaggingQueue.QueuedImages.Should().HaveCount(1);
                        tags.Should().HaveCountGreaterThan(1);
                });
                _imageTaggingQueue.QueuedImages.Should().BeEmpty();
        }
}
