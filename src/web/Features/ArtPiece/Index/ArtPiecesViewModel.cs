using System;

namespace web.Features.ArtPiece.Index;

public class ArtPiecesViewModel
{
        public List<ArtPiece> ArtPieces { get; }

        public ArtPiecesViewModel(List<ArtPiece> artPieces)
        {
                ArtPieces = artPieces;
        }
}
