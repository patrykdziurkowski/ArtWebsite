using System;

namespace web.features.artist;

public class ArtistProfileModel
{
        public Guid Id { get; }
        public string Name { get; }
        public string Summary { get; }
        public bool IsOwner { get; }

        public ArtistProfileModel(Guid id, string name, string summary, bool isOwner)
        {
                Id = id;
                Name = name;
                Summary = summary;
                IsOwner = isOwner;
        }

}
