namespace web.Features.Reviews;

public record Rating
{
        public int Value { get; }

        public Rating(int value)
        {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 5);
                Value = value;
        }


}
