using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Data;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.LoadArtPieces;

namespace tests.Integration.Queries;

[Collection("Database collection")]
public class ArtPiecesQueryTests : IDisposable
{
        private readonly ArtPiecesQuery _command;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;

        public ArtPiecesQueryTests(DatabaseTestContext databaseContext)
        {
                _scope = databaseContext.Services.CreateScope();
                _command = _scope.ServiceProvider.GetRequiredService<ArtPiecesQuery>();
                _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();
                _dbContext.Database.BeginTransaction();
        }

        public void Dispose()
        {
                _dbContext.Database.RollbackTransaction();
                _scope.Dispose();
        }

        [Fact]
        public void Execute_ShouldReturnEmpty_WhenNoArtPieces()
        {
                List<ArtPiece> artPieces = _command.Execute(new ArtistId(), 10);

                artPieces.Should().BeEmpty();
        }

        [Fact]
        public async Task Execute_ShouldReturnEmpty_WhenArtPieceExistsButByADifferentArtist()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                List<ArtPiece> artPieces = _command.Execute(new ArtistId(), 10);

                artPieces.Should().BeEmpty();
        }

        [Fact]
        public async Task Execute_ShouldReturn6ArtPieces_WhenArtistHas6ArtPieces()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                List<ArtPiece> artPieces = _command.Execute(artistId, 10);

                artPieces.Should().HaveCount(6);
        }

        [Fact]
        public async Task Execute_ShouldReturn1ArtPiece_GivenOffsetWhenArtistHas6ArtPieces()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                List<ArtPiece> artPieces = _command.Execute(artistId, 10, 5);

                artPieces.Should().HaveCount(1);
        }

        [Fact]
        public async Task Execute_ShouldReturn3ArtPieces_WhenAskedFor3AndArtistHas6()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                List<ArtPiece> artPieces = _command.Execute(artistId, 3, 0);

                artPieces.Should().HaveCount(3);
        }

        private async Task Create6ArtPiecesForArtist(ArtistId artistId)
        {
                for (int i = 0; i < 6; ++i)
                {
                        ArtPiece artPiece = new($"somePath{i}", "description", artistId);
                        await _dbContext.ArtPieces.AddAsync(artPiece);
                        await _dbContext.SaveChangesAsync();
                }
        }

        private async Task<ArtistId> CreateUserWithArtistProfile()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await _userManager.CreateAsync(user);
                ArtistId artistId = new();
                _dbContext.Artists.Add(
                        new Artist(artistId, user.Id, "ArtistName",
                                "A profile summary for an artist."));
                await _dbContext.SaveChangesAsync();
                return artistId;
        }
}
