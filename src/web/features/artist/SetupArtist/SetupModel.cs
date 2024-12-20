using System;

namespace web.features.artist.SetupArtist;

public class SetupModel(string name, string summary)
{
        public string Name { get; set; } = name;
        public string Summary { get; set; } = summary;
}
