using web.Features.Shared.domain;

namespace web.Features.Reviewers;

public class ReviewerId : DomainId
{
        public ReviewerId() : base(Guid.NewGuid())
        {
        }

        public ReviewerId(Guid guid) : base(guid)
        {
        }
}
