using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Missions;
using web.Features.Tags;

namespace tests.Integration.Queries;

public class TodaysMissionQueryTests : DatabaseTest
{
        private readonly TodaysMissionQuery _query;
        private readonly UploadArtPieceCommand _uploadArtPieceCommand;
        private readonly IMissionGenerator _mockMissionGenerator;

        public TodaysMissionQueryTests(DatabaseTestContext databaseContext) : base(databaseContext)
        {
                IServiceScopeFactory serviceScopeFactory = Scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
                ImageTaggingQueue imageTaggingQueue = Scope.ServiceProvider.GetRequiredService<ImageTaggingQueue>();
                ArtistRepository artistRepository = Scope.ServiceProvider.GetRequiredService<ArtistRepository>();
                _mockMissionGenerator = Substitute.For<IMissionGenerator>();
                MissionManager missionManager = new(DbContext, _mockMissionGenerator);
                _uploadArtPieceCommand = new UploadArtPieceCommand(DbContext, artistRepository, imageTaggingQueue, missionManager, serviceScopeFactory);
                _query = new(DbContext, _mockMissionGenerator);
        }

        [Fact]
        public async Task ExecuteAsync_Works()
        {
                _mockMissionGenerator.GetMissions(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>(), 1)
                        .Returns([MissionType.UploadArt]);
                await CreateUserWithArtistProfile();
                Guid currentUserId = DbContext.Users.Single().Id;
                _ = await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "some description", currentUserId);

                TodaysMissionDto dto = await _query.ExecuteAsync(currentUserId);

                dto.CurrentProgress.Should().Be(1);
                dto.MaxProgress.Should().Be(1);
                dto.Description.Should().Be(MissionType.UploadArt.GetDescription());
        }
}
