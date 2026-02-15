using FluentResults;
using Microsoft.AspNetCore.SignalR;
using web.Features.Tags;

namespace web.Features.Images;

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
                        ImageTaggingItem? imageTaggingItem = queue.TryDequeue();
                        if (imageTaggingItem is null)
                        {
                                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                                continue;
                        }

                        try
                        {
                                Result<List<string>> result = await imageTagger.TagImageAsync(imageTaggingItem.FullImagePath);
                                if (result.IsFailed)
                                {
                                        logger.LogInformation("Info: Trying to tag an image unsuccessful: {result}", result);
                                        continue;
                                }

                                logger.LogInformation($"INFO: Finished tagging an image in {DateTimeOffset.UtcNow.Subtract(imageTaggingItem.TimeStarted).TotalSeconds} seconds.");

                                List<string> tags = result.Value;
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
