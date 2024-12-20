using System;
using web.features.shared.domain;

namespace web.features.artist;

public class ArtistId(Guid value) : DomainId(value)
{
}
