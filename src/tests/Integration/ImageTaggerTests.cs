using FluentAssertions;
using web.Features.Tags.ImageRecognition;

namespace tests.Integration;

public class ImageTaggerTests
{
        [Fact]
        public async Task TagImage_ShouldReturnACoupleOfTags_ForImage()
        {
                ImageTagger imageTagger = new();
                string assemblyLocation = typeof(ImageTagger).Assembly.Location;
                string image = Path.GetFullPath(Path.Combine(
                        assemblyLocation,
                        "..",
                        "..",
                        "..",
                        "..",
                        "resources",
                        "exampleImage.png"));
                File.Exists(image).Should().BeTrue();

                List<string> tags = await imageTagger.TagImageAsync(image);

                tags.Count.Should().BeGreaterThan(1);
        }
}
