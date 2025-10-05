using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FluentAssertions;
using tests.Integration.Fixtures;

namespace tests.Integration;

public class AiApiTests(DatabaseTestContext databaseContext) : DatabaseBase(databaseContext)
{
        private const string URL = "http://localhost:8081/tag";

        [Fact]
        public async Task Tag_ShouldReturnListOfTags_WhenImageSuccessfullyTagged()
        {
                string existingImage = "../../../resources/exampleImage.png";

                HttpResponseMessage response = await RequestTagsForImageAsync(existingImage);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                        throw new InvalidOperationException($"Expected response to be OK, found {response.StatusCode} instead with message {await response.Content.ReadAsStringAsync()}");
                }

                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                List<string> results = JsonSerializer.Deserialize<List<string>>(json)
                        ?? throw new InvalidOperationException("Could not parse JSON response from tag server.");
                results.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public async Task Tag_ShouldReturnBadRequest_WhenNonImageSent()
        {
                string existingImage = "../../../resources/exampleNonImage.txt";

                HttpResponseMessage response = await RequestTagsForImageAsync(existingImage);

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private static async Task<HttpResponseMessage> RequestTagsForImageAsync(string imagePath)
        {
                if (!File.Exists(imagePath))
                {
                        throw new InvalidOperationException($"No file found at '{imagePath}'");
                }

                using HttpClient client = new();
                using MultipartFormDataContent content = [];
                using FileStream fileStream = File.OpenRead(imagePath);
                using StreamContent fileContent = new(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                content.Add(fileContent, "image", Path.GetFileName(imagePath));

                return await client.PostAsync(URL, content);
        }
}
