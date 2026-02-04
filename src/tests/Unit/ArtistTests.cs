using FluentAssertions;
using FluentResults;
using web.Features.Artists;
using web.Features.ArtPieces;

namespace tests.Unit;

public class ArtistTests
{
        [Fact]
        public void BoostArtPiece_ShouldReturnFail_WhenBoostAlreadyActive()
        {
                Artist artist = new()
                {
                        Name = "SomeArtist",
                        Summary = "My profile summary.",
                        UserId = Guid.NewGuid()
                };
                ArtPiece artPiece = new()
                {
                        ImagePath = "testPath",
                        Description = "some description",
                        ArtistId = artist.Id,
                };
                Result firstResult = artist.BoostArtPiece(artPiece.Id, artist.Id);

                Result secondResult = artist.BoostArtPiece(artPiece.Id, artist.Id);

                firstResult.IsSuccess.Should().BeTrue();
                secondResult.IsFailed.Should().BeTrue();
                artist.Points.Should().Be(20);
        }

        [Fact]
        public void BoostArtPiece_ShouldReturnFail_WhenArtPieceNotOwnedByCurrentArtist()
        {
                Artist artist = new()
                {
                        Name = "SomeArtist",
                        Summary = "My profile summary.",
                        UserId = Guid.NewGuid()
                };
                ArtPiece artPiece = new()
                {
                        ImagePath = "testPath",
                        Description = "some description",
                        ArtistId = new ArtistId(),
                };

                Result result = artist.BoostArtPiece(artPiece.Id, artPiece.ArtistId);

                result.IsFailed.Should().BeTrue();
                artist.Points.Should().Be(0);
        }

        [Fact]
        public void BoostArtPiece_ShouldReturnOkAndAddBoost_WhenValid()
        {
                Artist artist = new()
                {
                        Name = "SomeArtist",
                        Summary = "My profile summary.",
                        UserId = Guid.NewGuid()
                };
                ArtPiece artPiece = new()
                {
                        ImagePath = "testPath",
                        Description = "some description",
                        ArtistId = artist.Id,
                };

                Result result = artist.BoostArtPiece(artPiece.Id, artist.Id);

                result.IsSuccess.Should().BeTrue();
                artist.ActiveBoost.Should().NotBeNull();
                artist.Points.Should().Be(20);
        }


}
