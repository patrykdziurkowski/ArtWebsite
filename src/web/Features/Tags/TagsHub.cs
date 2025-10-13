using Microsoft.AspNetCore.SignalR;

namespace web.Features.Tags;

public class TagsHub : Hub
{
        public Task JoinArtGroup(Guid artPieceId)
        {
                return Groups.AddToGroupAsync(Context.ConnectionId, $"art-{artPieceId}");
        }

        public async Task SendTagsReady(Guid artPieceId, List<string> tags)
        {
                await Clients.Group($"art-{artPieceId}").SendAsync("TagsReady", tags);
        }
}
