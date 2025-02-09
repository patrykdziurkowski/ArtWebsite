using System;
using web.Features.Shared.domain;

namespace web.Features.Reviews;

public class ReviewId : DomainId
{
        public ReviewId() : base(Guid.NewGuid())
        {
        }

        public ReviewId(Guid guid) : base(guid)
        {
        }
}
