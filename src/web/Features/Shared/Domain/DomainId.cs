namespace web.Features.Shared.domain;

public abstract class DomainId : ValueObject
{
        public Guid Value { get; init; } = Guid.NewGuid();

        public override string ToString()
        {
                return Value.ToString();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
                yield return Value;
        }

}