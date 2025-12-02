using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Missions;
using web.Features.Tags;

namespace tests.Integration.Commands;

public class UploadArtPieceCommandTests : DatabaseTest
{
        private readonly UploadArtPieceCommand _command;

        public UploadArtPieceCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _command = new(
                        DbContext,
                        Scope.ServiceProvider.GetRequiredService<ArtistRepository>(),
                        Scope.ServiceProvider.GetRequiredService<ImageTaggingQueue>(),
                        Scope.ServiceProvider.GetRequiredService<MissionManager>(),
                        Scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>());
        }

        [Fact]
        public async Task ExecuteAsync_SavesImageObject()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                DbContext.Artists.Add(new Artist
                {
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                });

                await DbContext.SaveChangesAsync();


                ArtPiece artPiece = await _command.ExecuteAsync(
                        GetExampleFile(), "description", user.Id);

                DbContext.ArtPieces.FirstOrDefault(a => a.Description == "description")
                        .Should().NotBeNull();
                DbContext.Artists.First().Points.Should().Be(10);
                DbContext.ArtistPointAwards.Single().PointValue.Should().Be(10);
        }

        [Fact]
        public async Task ExecuteAsync_AddsExtraPoints_WhenUploadingArtPieceMissionCompleted()
        {
                const int POINTS_PER_UPLOAD = 10;
                const int POINTS_PER_QUEST = 25;

                for (int i = 0; i < 1000; i++)
                {
                        ClearDatabase();
                        IdentityUser<Guid> user = new("johnSmith");
                        await UserManager.CreateAsync(user);
                        DbContext.Artists.Add(new Artist
                        {
                                UserId = user.Id,
                                Name = "ArtistName",
                                Summary = "A profile summary for an artist.",
                        });
                        await DbContext.SaveChangesAsync();

                        _ = await _command.ExecuteAsync(
                                GetExampleFile(), "description", user.Id);

                        int points = DbContext.Artists.Single().Points;
                        if (points == POINTS_PER_UPLOAD + POINTS_PER_QUEST)
                        {
                                return;
                        }
                }

                throw new InvalidOperationException("Iteration limit exceeded. Test conditions not met.");
        }

        [Fact]
        public async Task ExecuteAsync_SavesImageAsAFile()
        {
                IdentityUser<Guid> user = new("johnSmith");
                await UserManager.CreateAsync(user);
                ArtistId artistId = new();
                DbContext.Artists.Add(new Artist
                {
                        Id = artistId,
                        UserId = user.Id,
                        Name = "ArtistName",
                        Summary = "A profile summary for an artist.",
                });
                await DbContext.SaveChangesAsync();

                ArtPiece artPiece = await _command.ExecuteAsync(
                        GetExampleFile(), "description", user.Id);

                string path = Path.Combine("user-images", "art-pieces",
                        $"{artistId}", $"{artPiece.Id}.png");
                File.Exists(path).Should().BeTrue();
        }



}
