using FluentAssertions;
using FluentResults;
using tests.Integration.Fixtures;
using web.Features.Tags;

namespace tests.Integration;

public class ImageTaggerTests(DatabaseTestContext databaseContext) : DatabaseTest(databaseContext)
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

                Result<List<string>> result = await imageTagger.TagImageAsync(image);

                result.IsFailed.Should().BeFalse();
                List<string> tags = result.Value;
                tags.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public void TagImage_ShouldThrow_WhenImageDoesntExist()
        {
                ImageTagger imageTagger = new();
                string image = "nonExistentImage.jpg";
                File.Exists(image).Should().BeFalse();

                Func<Task> callingTagImageAsync = async () => await imageTagger.TagImageAsync(image);

                callingTagImageAsync.Should().ThrowAsync<InvalidOperationException>();
        }

}
