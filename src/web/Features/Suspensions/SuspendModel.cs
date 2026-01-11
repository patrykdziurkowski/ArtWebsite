using System.ComponentModel.DataAnnotations;

namespace web.Features.Suspensions;

public record SuspendModel
{
        [Range(1, int.MaxValue, ErrorMessage = "The duration value must be greater than 0.")]
        public required int DurationMinutes { get; init; }

        public required string Reason { get; init; }
}
