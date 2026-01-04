namespace web.Features.Reviewers.Index;

public record ReviewerProfileDto
{
        public required ReviewerId Id { get; init; }
        public required string Name { get; set; }
        public required DateTimeOffset JoinDate { get; init; }
        public required int ReviewCount { get; init; }
        public required int Points { get; init; }
        public required bool CurrentUserOwnsThisProfile { get; init; }
        public required bool IsCurrentUserAdmin { get; init; }

}
