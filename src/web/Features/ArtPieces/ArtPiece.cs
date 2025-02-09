using web.Features.Artists;
using web.Features.Shared.domain;

namespace web.Features.ArtPieces;

public class ArtPiece : AggreggateRoot
{
        public ArtPieceId Id { get; }
        public string ImagePath { get; }
        public string Description { get; set; }
        public DateTime UploadDate { get; }
        public ArtistId ArtistId { get; }

        private ArtPiece()
        {
                Id = new ArtPieceId();
                ImagePath = string.Empty;
                Description = string.Empty;
                UploadDate = DateTime.UtcNow;
                ArtistId = new ArtistId();
        }

        public ArtPiece(ArtPieceId id, string imagePath,
                string description, DateTime uploadDate,
                ArtistId artistId)
        {
                Id = id;
                ImagePath = imagePath;
                Description = description;
                UploadDate = uploadDate;
                ArtistId = artistId;
        }

        public ArtPiece(string imagePath,
                string description,
                ArtistId artistId)
        {
                Id = new ArtPieceId();
                ImagePath = imagePath;
                Description = description;
                UploadDate = DateTime.UtcNow;
                ArtistId = artistId;
        }

}
