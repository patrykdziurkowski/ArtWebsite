using web.Features.Shared.domain;

namespace web.Features.Tags;

public class Tag : AggregateRoot
{
        public TagId Id { get; init; } = new();
        public required string Name { get; init; }
}
