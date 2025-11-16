namespace web.Features.Reviews;

public record Rating
{
        public int Value { get; private init; }

        public Rating(int value)
        {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 5);
                Value = value;
        }

        public Rating(double value)
        {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 5);
                Value = (int)double.Round(value);
        }

        private Rating()
        {
                Value = 0;
        }

        public static Rating Empty => new();

        public static implicit operator int(Rating rating) => rating.Value;
        public static implicit operator Rating(int value) => new(value);
}
