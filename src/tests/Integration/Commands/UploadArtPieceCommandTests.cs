using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Images;
using web.Features.Missions;

namespace tests.Integration.Commands;

public class UploadArtPieceCommandTests : DatabaseTest
{
        private readonly UploadArtPieceCommand _command;
        private readonly IMissionGenerator _mockMissionGenerator;

        public UploadArtPieceCommandTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                _mockMissionGenerator = Substitute.For<IMissionGenerator>();
                MissionManager missionManager = new(DbContext, _mockMissionGenerator);
                _command = new(
                        DbContext,
                        Scope.ServiceProvider.GetRequiredService<ArtistRepository>(),
                        Scope.ServiceProvider.GetRequiredService<ImageTaggingQueue>(),
                        missionManager,
                        Scope.ServiceProvider.GetRequiredService<ImageManager>(),
                        Scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>());
        }

        [Fact]
        public async Task ExecuteAsync_SavesImageObject()
        {
                _mockMissionGenerator.GetMissions(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>(), 1)
                        .Returns([MissionType.BoostArt]);
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
                _mockMissionGenerator.GetMissions(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>(), 1)
                        .Returns([MissionType.UploadArt]);
                const int POINTS_PER_UPLOAD = 10;
                const int POINTS_PER_QUEST = 25;

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

                DbContext.Artists.Single().Points.Should().Be(POINTS_PER_UPLOAD + POINTS_PER_QUEST);
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

                string path = $"./user-images/art-pieces/{artistId}/{artPiece.Id}.png";

                File.Exists(path).Should().BeTrue();
        }



}
