using System;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.integration.fixtures;
using web.data;
using web.features.artist;
using web.Features.ArtPiece;
using web.Features.ArtPiece.Index;

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
        public async Task ExecuteAsync_ShouldReturnEmpty_WhenNoArtPiecesInDatabase()
        {
                List<ArtPiece> artPieces = await _command.ExecuteAsync(5);

                artPieces.Count.Should().Be(0);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturn5Results_When6Exist()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);

                List<ArtPiece> artPieces = await _command.ExecuteAsync(5);

                artPieces.Count.Should().Be(5);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturn3NewerResults_When6Exist()
        {
                ArtistId artistId = await CreateUserWithArtistProfile();
                await Create6ArtPiecesForArtist(artistId);
                List<ArtPiece> totalArtPieces = await _command.ExecuteAsync(6);
                DateTime lastArtPieceUploadDate = totalArtPieces.ElementAt(2).UploadDate;

                List<ArtPiece> artPieces = await _command.ExecuteAsync(5, lastArtPieceUploadDate);

                artPieces.Count.Should().Be(3);
                artPieces.Any(a => a.UploadDate == lastArtPieceUploadDate).Should().BeFalse();
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
