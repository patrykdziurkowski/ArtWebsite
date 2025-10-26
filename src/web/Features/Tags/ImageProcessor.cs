using Microsoft.AspNetCore.SignalR;

namespace web.Features.Tags;

public class ImageProcessor(
        ImageTaggingQueue queue,
        ImageTagger imageTagger,
        ILogger<ImageProcessor> logger,
        IHubContext<TagsHub> tagsHub)
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
                                logger.LogDebug("Sending TagsReady to group: {group}", $"art-{imageTaggingItem.ArtPieceId}");
                                await tagsHub.Clients.Group($"art-{imageTaggingItem.ArtPieceId}").SendAsync("TagsReady", tags, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                                string message = ex.Message.ToString();
                                logger.LogError(ex, "An error occurred when trying to tag an image: {message}", message);
                        }
                }
        }
}
