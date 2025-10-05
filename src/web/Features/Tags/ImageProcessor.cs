namespace web.Features.Tags;

public class ImageProcessor(
        ImageTaggingQueue queue,
        ImageTagger imageTagger,
        ILogger<ImageProcessor> logger)
        : BackgroundService
{
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                while (!stoppingToken.IsCancellationRequested)
                {
                        ImageTaggingItem? imageTaggingItem = queue.Dequeue();
                        if (imageTaggingItem is null)
                        {
                                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                                continue;
                        }

                        try
                        {
                                List<string> tags = await imageTagger.TagImageAsync(imageTaggingItem.FullImagePath);
                                await imageTaggingItem.CallBack(tags);
                        }
                        catch (Exception ex)
                        {
                                string message = ex.Message.ToString();
                                logger.LogError(ex, "An error occurred when trying to tag an image: {message}", message);
                        }
                }
        }
}
