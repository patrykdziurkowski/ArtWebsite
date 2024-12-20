using web.features.shared.domain;

namespace web.features.artist;

public class Artist : AggreggateRoot
{
        public ArtistId ArtistId { get; }
        public string Name { get; set; }
        public string Summary { get; set; }

        public Artist(ArtistId artistId, string name, string summary)
        {
                ArtistId = artistId;
                Name = name;
                Summary = summary;
        }
}
