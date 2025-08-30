using FluentAssertions;
using web.Features.Tags.ImageRecognition;

namespace tests.Integration;

public class ImageTaggingQueueTests
{
        private readonly ImageTaggingQueue _imageTaggingQueue = new(new ImageTagger());
        private readonly string _image;

        public ImageTaggingQueueTests()
        {
                string assemblyLocation = typeof(ImageTagger).Assembly.Location;
                _image = Path.GetFullPath(Path.Combine(
                        assemblyLocation,
                        "..",
                        "..",
                        "..",
                        "..",
                        "resources",
                        "exampleImage.png"));
                File.Exists(_image).Should().BeTrue();

        }

        [Fact]
        public async Task Add_ShouldThrow_WhenSameImageQueuedTwice()
        {
                _ = _imageTaggingQueue.Add(_image, (tags) => { });
                Func<Task> queuingImage = async () => await _imageTaggingQueue.Add(_image, (tags) => { });

                await queuingImage.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Add_ShouldWork()
        {
                await _imageTaggingQueue.Add(_image, (tags) =>
                {
                        _imageTaggingQueue.QueuedImages.Should().HaveCount(1);
                        tags.Should().HaveCountGreaterThan(1);
                });
                _imageTaggingQueue.QueuedImages.Should().BeEmpty();
        }
}
