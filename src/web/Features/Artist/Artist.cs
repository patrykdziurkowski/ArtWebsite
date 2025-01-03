using web.features.shared.domain;

namespace web.features.artist;

public class Artist : AggreggateRoot
{
        public ArtistId Id { get; }
        public Guid OwnerId { get; }
        public string Name { get; set; }
        public string Summary { get; set; }

        private Artist()
        {
                Id = new ArtistId(Guid.Empty);
                OwnerId = Guid.Empty;
                Name = string.Empty;
                Summary = string.Empty;
        }

        public Artist(ArtistId artistId, Guid ownerId,
                string name, string summary)
        {
                Id = artistId;
                OwnerId = ownerId;
                Name = name;
                Summary = summary;
        }

        public Artist(Guid ownerId, string name, string summary)
                : this(new ArtistId(), ownerId, name, summary)
        {
        }
}
