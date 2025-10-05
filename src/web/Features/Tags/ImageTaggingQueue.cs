using System.Collections.Concurrent;

namespace web.Features.Tags;

public class ImageTaggingQueue
{
        public ConcurrentQueue<ImageTaggingItem> QueuedImages { get; } = [];

        public void Add(string fullImagePath, Func<List<string>, Task> callBack)
        {
                QueuedImages.Enqueue(new()
                {
                        FullImagePath = fullImagePath,
                        CallBack = callBack,
                });
        }

        public ImageTaggingItem? Dequeue()
        {
                bool itemDequeued = QueuedImages.TryDequeue(out ImageTaggingItem? result);
                return itemDequeued ? result : null;
        }
}

public record ImageTaggingItem
{
        public required string FullImagePath { get; init; }
        public required Func<List<string>, Task> CallBack { get; init; }
}
