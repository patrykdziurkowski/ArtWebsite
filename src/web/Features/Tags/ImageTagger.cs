using System.Text.Json;

namespace web.Features.Tags;

public class ImageTagger
{
        public string Url { get; }

        public ImageTagger()
        {
                string url = Environment.GetEnvironmentVariable("AI_CONTAINER_URL")
                        ?? "http://ai:8081/";
                url = url.TrimEnd('/');
                Url = $"{url}/tag";
        }

        public async Task<List<string>> TagImageAsync(string fullImagePath)
        {
                if (!File.Exists(fullImagePath))
                {
                        throw new InvalidOperationException($"No image with path '{fullImagePath}' found when trying to generate tags for it.");
                }

                using HttpClient client = new();
                using MultipartFormDataContent content = [];
                using FileStream fileStream = File.OpenRead(fullImagePath);
                content.Add(new StreamContent(fileStream), "image", fullImagePath);

                HttpResponseMessage response = await client.PostAsync(Url, content);
                if (!response.IsSuccessStatusCode)
                {
                        throw new InvalidOperationException($"Unable to fetch tags for image. Status {response.StatusCode} with message '{await response.Content.ReadAsStringAsync()}'");
                }

                string json = await response.Content.ReadAsStringAsync();
                List<string> tags = JsonSerializer.Deserialize<List<string>>(json)
                        ?? throw new InvalidOperationException("Could not parse JSON response from tag server.");
                return tags;
        }
}
