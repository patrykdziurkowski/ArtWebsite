using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.integration.fixtures;
using web.data;
using web.features.artist;
using web.Features.ArtPiece;
using web.Features.ArtPiece.Index;

namespace tests.Integration.Queries;

[Collection("Database collection")]
public class ArtPieceQueryTests : IDisposable
{
        private readonly ArtPieceQuery _command;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;

        public ArtPieceQueryTests(DatabaseTestContext databaseContext)
        {
                _scope = databaseContext.Services.CreateScope();
                _command = _scope.ServiceProvider.GetRequiredService<ArtPieceQuery>();
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
        public void Execute_ShouldReturnNull_WhenNoArtPiecesInDatabase()
        {
                ArtPiece? artPiece = _command.Execute();

                artPiece.Should().BeNull();
        }

        [Fact]
        public async Task Execute_ShouldReturnAnArtPiece_WhenOneExists()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                ArtPiece? artPiece = _command.Execute();

                artPiece.Should().NotBeNull();
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
