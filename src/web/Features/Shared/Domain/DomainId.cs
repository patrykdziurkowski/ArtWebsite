using System;

namespace web.Features.Shared.domain;

public abstract class DomainId(Guid value) : ValueObject
{
        public Guid Value { get; init; } = value;

        public override string ToString()
        {
                return Value.ToString();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
                yield return Value;
        }

}