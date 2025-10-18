using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;

namespace tests.Integration;

public class ArtistRepositoryTests : DatabaseTest
{
        private readonly ArtistRepository _artistRepository;

        public ArtistRepositoryTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _artistRepository = Scope.ServiceProvider.GetRequiredService<ArtistRepository>();
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnNull_WhenArtistNotFound()
        {
                Artist? artist = await _artistRepository.GetByNameAsync("NonExistant");

                artist.Should().BeNull();
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnArtist_WhenExists()
        {
                await CreateArtistUserWithArtPieces();

                Artist? queriedArtist = await _artistRepository.GetByNameAsync("ArtistName");

                queriedArtist.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnArtistWithActiveBoost_WhenExists()
        {
                ArtPieceId artPieceId = (await CreateArtistUserWithArtPieces()).First();
                Artist? artist = await _artistRepository.GetByNameAsync("ArtistName");
                artist!.BoostArtPiece(artPieceId, artist.Id);
                await DbContext.SaveChangesAsync();

                Artist? queriedArtist = await _artistRepository.GetByNameAsync("ArtistName");

                queriedArtist.Should().NotBeNull();
                queriedArtist.ActiveBoost!.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnArtistWithInactiveBoost_WhenNoActiveBoosts()
        {
                ArtPieceId artPieceId = (await CreateArtistUserWithArtPieces()).First();
                Artist? artist = await _artistRepository.GetByNameAsync("ArtistName");
                Boost boost = new()
                {
                        ArtistId = artist!.Id,
                        ArtPieceId = artPieceId,
                        Date = DateTimeOffset.UtcNow.AddDays(-5),
                        ExpirationDate = DateTimeOffset.UtcNow.AddDays(-4),
                };
                DbContext.Add(boost);
                await DbContext.SaveChangesAsync();

                Artist? queriedArtist = await _artistRepository.GetByNameAsync("ArtistName");

                queriedArtist!.ActiveBoost.Should().BeNull();
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnArtistWithActiveBoost_WhenTwoBoostsAndOneIsActive()
        {
                ArtPieceId artPieceId = (await CreateArtistUserWithArtPieces()).First();
                Artist? artist = await _artistRepository.GetByNameAsync("ArtistName");
                Boost boost = new()
                {
                        ArtistId = artist!.Id,
                        ArtPieceId = artPieceId,
                        Date = DateTimeOffset.UtcNow.AddDays(-5),
                        ExpirationDate = DateTimeOffset.UtcNow.AddDays(-4),
                };
                DbContext.Add(boost);
                Boost boost2 = new()
                {
                        ArtistId = artist!.Id,
                        ArtPieceId = artPieceId,
                };
                DbContext.Add(boost2);
                await DbContext.SaveChangesAsync();

                Artist? queriedArtist = await _artistRepository.GetByNameAsync("ArtistName");

                queriedArtist!.ActiveBoost.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenArtistNotFound()
        {
                Artist? artist = await _artistRepository.GetByIdAsync(new ArtistId());

                artist.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnArtist_WhenArtistFound()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();

                Artist? artist = await _artistRepository.GetByIdAsync(artistId);

                artist.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnNull_WhenNoArtistForUser()
        {
                IdentityUser<Guid> user = new("userName");
                await UserManager.CreateAsync(user);

                Artist? artist = await _artistRepository.GetByUserIdAsync(user.Id);

                artist.Should().BeNull();
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnArtist_WhenExists()
        {
                IdentityUser<Guid> user = new("userName");
                await UserManager.CreateAsync(user);
                Artist artist = new()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                };
                await _artistRepository.SaveChangesAsync(artist);

                Artist? queriedArtist = await _artistRepository.GetByUserIdAsync(user.Id);

                queriedArtist.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_SavesChanges_WhenCalled()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                Artist? artist = await _artistRepository.GetByIdAsync(artistId);

                artist!.Name = "NewName";
                await _artistRepository.SaveChangesAsync(artist);

                Artist? changedArtist = await _artistRepository.GetByIdAsync(artistId);
                changedArtist!.Name.Should().Be("NewName");
        }

        [Fact]
        public async Task SaveChangesAsync_CreatesNewArtist_WhenNewArtistPassedAsParameter()
        {
                IdentityUser<Guid> user = new("userName");
                await UserManager.CreateAsync(user);
                Artist artist = new()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                };

                await _artistRepository.SaveChangesAsync(artist);

                Artist? queriedArtist = await _artistRepository.GetByNameAsync("ArtistName");
                queriedArtist.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_DoesntCreateAnotherArtist_WhenAlreadyTrackedButIsPassedAsParameter()
        {
                IdentityUser<Guid> user = new("userName");
                await UserManager.CreateAsync(user);
                Artist artist = new()
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                };
                await _artistRepository.SaveChangesAsync(artist);

                await _artistRepository.SaveChangesAsync(artist);

                Artist? queriedArtist = await _artistRepository.GetByNameAsync("ArtistName");
                queriedArtist.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_SavesArtistsBoost_WhenCalled()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);
                Artist? artist = await _artistRepository.GetByIdAsync(artistId);
                ArtPieceId artPieceId = DbContext.ArtPieces.First().Id;

                artist!.BoostArtPiece(artPieceId, artistId);
                await _artistRepository.SaveChangesAsync(artist);

                Artist? changedArtist = await _artistRepository.GetByIdAsync(artistId);
                changedArtist!.ActiveBoost.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_DeletesArtist_WhenDeactivated()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);
                Artist? artist = await _artistRepository.GetByIdAsync(artistId);

                artist!.Deactivate();
                await _artistRepository.SaveChangesAsync(artist);

                Artist? deactivatedArtist = await _artistRepository.GetByIdAsync(artistId);
                deactivatedArtist.Should().BeNull();
        }

}
