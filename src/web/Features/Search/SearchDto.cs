using web.Features.Artists;
using web.Features.Reviewers;
using web.Features.Tags;

namespace web.Features.Search;

public record SearchDto
{
        public required List<Tag> Tags { get; init; }
        public required List<Artist> Artists { get; init; }
        public required List<Reviewer> Reviewers { get; init; }
}
