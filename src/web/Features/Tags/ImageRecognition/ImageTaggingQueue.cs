namespace web.Features.Tags.ImageRecognition;

public class ImageTaggingQueue(ImageTagger imageTagger)
{
        private readonly ImageTagger _imageTagger = imageTagger;
        public Dictionary<string, Action<List<string>>> QueuedImages { get; } = [];

        public async Task Add(string fullImagePath, Action<List<string>> callBack)
        {
                if (QueuedImages.ContainsKey(fullImagePath))
                {
                        throw new InvalidOperationException($"Could not queue image with path '{fullImagePath}' since it already is being processed.");
                }

                QueuedImages.Add(fullImagePath, callBack);
                List<string> tags = await _imageTagger.TagImageAsync(fullImagePath);
                callBack(tags);
                QueuedImages.Remove(fullImagePath);
        }
}
