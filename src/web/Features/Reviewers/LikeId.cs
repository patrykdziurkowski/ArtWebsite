using web.Features.Shared.domain;

namespace web.Features.Reviewers;

public class LikeId : DomainId
{
        public LikeId() : base(Guid.NewGuid())
        {
        }

        public LikeId(Guid guid) : base(guid)
        {
        }
}
