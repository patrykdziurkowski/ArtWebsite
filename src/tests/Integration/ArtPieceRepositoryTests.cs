using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using tests.Integration.Fixtures;
using web.Features.Artists;
using web.Features.Artists.BoostArtPiece;
using web.Features.ArtPieces;
using web.Features.ArtPieces.UploadArtPiece;
using web.Features.Missions;
using web.Features.Reviewers;
using web.Features.Reviews;
using web.Features.Tags;
using Xunit.Abstractions;

namespace tests.Integration;

public class ArtPieceRepositoryTests : DatabaseTest
{
        private readonly ArtPieceRepository _artPieceRepository;

        private readonly UploadArtPieceCommand _uploadArtPieceCommand;
        private readonly BoostArtPieceCommand _boostArtPieceCommand;
        private readonly IMissionGenerator _mockMissionGenerator;
        private readonly ITestOutputHelper _output;

        public ArtPieceRepositoryTests(DatabaseTestContext databaseContext, ITestOutputHelper output)
                : base(databaseContext)
        {
                _output = output;

                _artPieceRepository = Scope.ServiceProvider.GetRequiredService<ArtPieceRepository>();
                ArtistRepository artistRepository = Scope.ServiceProvider.GetRequiredService<ArtistRepository>();
                IServiceScopeFactory serviceScopeFactory = Scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
                ImageTaggingQueue imageTaggingQueue = Scope.ServiceProvider.GetRequiredService<ImageTaggingQueue>();
                IMapper mapper = Scope.ServiceProvider.GetRequiredService<IMapper>();
                _mockMissionGenerator = Substitute.For<IMissionGenerator>();
                MissionManager missionManager = new(DbContext, _mockMissionGenerator);
                _uploadArtPieceCommand = new UploadArtPieceCommand(DbContext, artistRepository, imageTaggingQueue, missionManager, serviceScopeFactory);
                _boostArtPieceCommand = new BoostArtPieceCommand(artistRepository, DbContext, mapper, missionManager);
        }

        [Fact]
        public async Task GetByAlgorithm_LowScoreArtPieceCanUpsetHighScoreArtPiece_WhenLucky()
        {
                // ARRANGE
                // Popular art piece - artist has high points, uploaded recently, high average rating, boosted.
                _mockMissionGenerator.GetMissions(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>(), 1)
                        .Returns([MissionType.ReviewArt]);
                ArtistId popularArtistId = await CreateUserWithArtistProfile("popularArtist", "popularArtist");
                Artist popularArtist = DbContext.Artists.First(a => a.Id == popularArtistId);
                popularArtist.Points = 10000;
                await DbContext.SaveChangesAsync();
                ArtPiece popularArtPiece = await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", popularArtist.UserId);
                popularArtPiece.AverageRating = new Rating(5);
                await DbContext.SaveChangesAsync();
                await _boostArtPieceCommand.ExecuteAsync(popularArtist.UserId, popularArtPiece.Id);

                // Unpopular art piece - artist has no points, uploaded a long time ago, average rating of 0, not boosted.
                ArtistId unpopularArtistId = await CreateUserWithArtistProfile("unpopularArtist", "unpopularArtist");
                Artist unpopularArtist = DbContext.Artists.First(a => a.Id == unpopularArtistId);
                ArtPiece unpopularArtPiece = await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", unpopularArtist.UserId);
                typeof(ArtPiece).GetProperty(nameof(ArtPiece.UploadDate))!
                        .SetValue(unpopularArtPiece, new DateTimeOffset(new DateTime(1999, 1, 1)));
                await DbContext.SaveChangesAsync();

                // ACT
                ReviewerId reviewerId = await CreateReviewer();
                bool unpopularArtPieceServed = false;
                for (int i = 0; i < 100000; i++)
                {
                        ArtPiece? artPiece = await _artPieceRepository.GetByAlgorithmAsync(reviewerId);
                        artPiece.Should().NotBeNull();
                        if (artPiece.Id == unpopularArtPiece.Id)
                        {
                                unpopularArtPieceServed = true;
                                _output.WriteLine($"SUCCESS: An unpopular art piece was served instead of the popular one after {i} failed attempts.");
                                break;
                        }
                }

                // ASSERT
                unpopularArtPieceServed.Should().BeTrue();

                popularArtist = await DbContext.Artists.FirstAsync(a => a.Id == popularArtistId);
                popularArtist.Points.Should().Be(10010);
                popularArtist.ActiveBoost.Should().NotBeNull();
                popularArtist.ActiveBoost.ArtPieceId.Should().Be(popularArtPiece.Id);
                DbContext.ArtPieces.First(ap => ap.Id == popularArtPiece.Id).AverageRating.Should().Be(new Rating(5));

                unpopularArtist = await DbContext.Artists.FirstAsync(a => a.Id == unpopularArtistId);
                unpopularArtist.Points.Should().Be(10);
                unpopularArtist.ActiveBoost.Should().BeNull();
                DbContext.ArtPieces.First(ap => ap.Id == unpopularArtPiece.Id).AverageRating.Should().Be(Rating.Empty);
        }

#pragma warning disable xUnit1004 // Test methods should not be skipped
        [Theory(Skip = "Due to the indeterministic nature of this test, it's meant to be ran manually and have its results verified by eye.")]
        // [Theory]
        [InlineData(5, 5, 5, 5)]
        [InlineData(1, 0, 0, 100)]
        [InlineData(5, 10, 25, 100)]
        [InlineData(5, 25, 100, 1000)]
        public async Task GetByAlgorithm_GivenManyDifferentArtPieces_OrdersThemProperly(
                int veryPopularArtPiecesCount,
                int popularArtPiecesCount,
                int barelyPopularArtPiecesCount,
                int unpopularArtPiecesCount)
        {
                const int SAMPLE_SIZE = 1000;
                ReviewerId reviewerId = await CreateReviewer();

                ArtPieceArchetypes artPieceArchetypes = await SeedArtPiecesAsync(
                        veryPopularArtPiecesCount, popularArtPiecesCount,
                        barelyPopularArtPiecesCount, unpopularArtPiecesCount);

                _output.WriteLine($"Fetching {SAMPLE_SIZE} art pieces...");
                for (int i = 0; i < SAMPLE_SIZE; i++)
                {
                        ArtPiece? artPiece = await _artPieceRepository.GetByAlgorithmAsync(reviewerId);
                        artPiece.Should().NotBeNull();
                        _output.WriteLine($"Art piece {i + 1}: {artPieceArchetypes.GetArchetype(artPiece.Id)}");
                }
                // view test output to analyze the results
        }
#pragma warning restore xUnit1004 // Test methods should not be skipped

        private async Task<ArtPieceArchetypes> SeedArtPiecesAsync(
                int veryPopularArtPiecesCount,
                int popularArtPiecesCount,
                int barelyPopularArtPiecesCount,
                int unpopularArtPiecesCount)
        {
                Dictionary<ArtPieceId, ArtPieceArchetype> artPieceArchetypes = [];

                for (int i = 0; i < veryPopularArtPiecesCount; i++)
                {
                        ArtistId veryPopularArtistId = await CreateUserWithArtistProfile($"verypopularArtist{i}", $"verypopularArtist{i}");
                        Artist veryPopularArtist = DbContext.Artists.First(a => a.Id == veryPopularArtistId);
                        veryPopularArtist.Points = 10000;
                        await DbContext.SaveChangesAsync();
                        ArtPiece veryPopularArtPiece = await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", veryPopularArtist.UserId);
                        veryPopularArtPiece.AverageRating = new Rating(5);
                        await DbContext.SaveChangesAsync();
                        await _boostArtPieceCommand.ExecuteAsync(veryPopularArtist.UserId, veryPopularArtPiece.Id);
                        artPieceArchetypes.Add(veryPopularArtPiece.Id, ArtPieceArchetype.VERY_POPULAR);
                }

                for (int i = 0; i < popularArtPiecesCount; i++)
                {
                        ArtistId popularArtistId = await CreateUserWithArtistProfile($"popularArtist{i}", $"popularArtist{i}");
                        Artist popularArtist = DbContext.Artists.First(a => a.Id == popularArtistId);
                        ArtPiece popularArtPiece = await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", popularArtist.UserId);
                        popularArtist.Points = 7000;
                        popularArtPiece.AverageRating = new Rating(4);
                        typeof(ArtPiece).GetProperty(nameof(ArtPiece.UploadDate))!
                               .SetValue(popularArtPiece, popularArtPiece.UploadDate.Subtract(TimeSpan.FromDays(2)));
                        await DbContext.SaveChangesAsync();
                        artPieceArchetypes.Add(popularArtPiece.Id, ArtPieceArchetype.POPULAR);
                }

                for (int i = 0; i < barelyPopularArtPiecesCount; i++)
                {
                        ArtistId barelyPopularArtistId = await CreateUserWithArtistProfile($"barelypopularartist{i}", $"barelypopularartist{i}");
                        Artist barelyPopularArtist = DbContext.Artists.First(a => a.Id == barelyPopularArtistId);
                        ArtPiece barelyPopularArtPiece = await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", barelyPopularArtist.UserId);
                        barelyPopularArtist.Points = 1000;
                        barelyPopularArtPiece.AverageRating = new Rating(2);
                        typeof(ArtPiece).GetProperty(nameof(ArtPiece.UploadDate))!
                               .SetValue(barelyPopularArtPiece, barelyPopularArtPiece.UploadDate.Subtract(TimeSpan.FromDays(10)));
                        await DbContext.SaveChangesAsync();
                        artPieceArchetypes.Add(barelyPopularArtPiece.Id, ArtPieceArchetype.BARELY_POPULAR);
                }

                for (int i = 0; i < unpopularArtPiecesCount; i++)
                {
                        ArtistId unpopularArtistId = await CreateUserWithArtistProfile($"unpopularArtist{i}", $"unpopularArtist{i}");
                        Artist unpopularArtist = DbContext.Artists.First(a => a.Id == unpopularArtistId);
                        ArtPiece unpopularArtPiece = await _uploadArtPieceCommand.ExecuteAsync(GetExampleFile(), "description", unpopularArtist.UserId);
                        typeof(ArtPiece).GetProperty(nameof(ArtPiece.UploadDate))!
                                .SetValue(unpopularArtPiece, new DateTimeOffset(new DateTime(1999, 1, 1)));
                        await DbContext.SaveChangesAsync();
                        artPieceArchetypes.Add(unpopularArtPiece.Id, ArtPieceArchetype.UNPOPULAR);
                }

                return new ArtPieceArchetypes(artPieceArchetypes);
        }

        private class ArtPieceArchetypes(Dictionary<ArtPieceId, ArtPieceArchetype> artPieces)
        {
                private readonly Dictionary<ArtPieceId, ArtPieceArchetype> _artPieces = artPieces;

                public ArtPieceArchetype GetArchetype(ArtPieceId artPieceId)
                {
                        return _artPieces[artPieceId];
                }
        }

        private enum ArtPieceArchetype
        {
                VERY_POPULAR,
                POPULAR,
                BARELY_POPULAR,
                UNPOPULAR,
        }
}
