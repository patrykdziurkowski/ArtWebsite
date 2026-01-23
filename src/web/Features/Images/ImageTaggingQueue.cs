using System.Collections.Concurrent;
using web.Features.ArtPieces;

namespace web.Features.Images;

public class ImageTaggingQueue
{
        public ConcurrentQueue<ImageTaggingItem> QueuedImages { get; } = [];

        public void Add(ArtPieceId artPieceId, string fullImagePath, Func<List<string>, Task> callBack)
        {
                if (!File.Exists(fullImagePath))
                {
                        throw new InvalidOperationException("Unable to queue an image to be tagged! The file doesn't exist.");
                }

                QueuedImages.Enqueue(new()
                {
                        ArtPieceId = artPieceId,
                        FullImagePath = fullImagePath,
                        CallBack = callBack,
                });
        }

        public ImageTaggingItem? TryDequeue()
        {
                bool itemDequeued = QueuedImages.TryDequeue(out ImageTaggingItem? result);
                return itemDequeued ? result : null;
        }
}

public record ImageTaggingItem
{
        public required ArtPieceId ArtPieceId { get; init; }
        public required string FullImagePath { get; init; }
        public required Func<List<string>, Task> CallBack { get; init; }
}
