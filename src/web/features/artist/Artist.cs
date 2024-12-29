using web.features.shared.domain;

namespace web.features.artist;

public class Artist : AggreggateRoot
{
        public ArtistId ArtistId { get; }
        public string OwnerId { get; }
        public string Name { get; set; }
        public string Summary { get; set; }

        private Artist()
        {
                ArtistId = new ArtistId(Guid.Empty);
                OwnerId = string.Empty;
                Name = string.Empty;
                Summary = string.Empty;
        }

        public Artist(ArtistId artistId, string ownerId,
                string name, string summary)
        {
                ArtistId = artistId;
                OwnerId = ownerId;
                Name = name;
                Summary = summary;
        }

        public Artist(string ownerId, string name, string summary)
                : this(new ArtistId(), ownerId, name, summary)
        {
        }
}
