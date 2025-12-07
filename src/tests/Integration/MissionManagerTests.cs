using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using tests.Integration.Fixtures;
using web.Features.Missions;

namespace tests.Integration;

public class MissionManagerTests : DatabaseTest
{
        private readonly MissionManager missionManager;

        public MissionManagerTests(DatabaseTestContext databaseContext)
                : base(databaseContext)
        {
                missionManager = Scope.ServiceProvider.GetRequiredService<MissionManager>();
        }

        [Theory]
        [InlineData(MissionType.UploadArt)]
        [InlineData(MissionType.BoostArt)]
        [InlineData(MissionType.ReviewArt)]
        [InlineData(MissionType.LikeArt)]
        [InlineData(MissionType.VisitArtistsProfiles)]
        [InlineData(MissionType.VisitReviewersProfiles)]
        public async Task RecordProgress_ShouldNotAwardPoints_ForANonCompleteMission(MissionType missionType)
        {
                await CreateUserWithArtistProfile();
                Guid userId = DbContext.Users.Single().Id;
                DateTimeOffset date = GetDateForUserThatGeneratesThisMission(userId, missionType);
                int artistPointsBefore = DbContext.Artists.Single().Points;
                int reviewerPointsBefore = DbContext.Reviewers.Single().Points;

                for (int i = 0; i < missionType.GetMaxProgressCount() - 1; i++)
                {
                        await missionManager.RecordProgressAsync(missionType, userId, date);
                }

                int artistPointsAfter = DbContext.Artists.Single().Points;
                int reviewerPointsAfter = DbContext.Reviewers.Single().Points;
                (artistPointsAfter - artistPointsBefore).Should().Be(0);
                (reviewerPointsAfter - reviewerPointsBefore).Should().Be(0);
        }

        [Theory]
        [InlineData(MissionType.UploadArt, 25, 0)]
        [InlineData(MissionType.BoostArt, 25, 0)]
        [InlineData(MissionType.ReviewArt, 0, 25)]
        [InlineData(MissionType.LikeArt, 0, 25)]
        [InlineData(MissionType.VisitArtistsProfiles, 25, 25)]
        [InlineData(MissionType.VisitReviewersProfiles, 25, 25)]
        public async Task RecordProgress_ShouldAwardPoints_ForACompletedMission(
                MissionType missionType,
                int expectedArtistPointsIncrease,
                int expectedReviewerPointsIncrease)
        {
                await CreateUserWithArtistProfile();
                Guid userId = DbContext.Users.Single().Id;
                DateTimeOffset date = GetDateForUserThatGeneratesThisMission(userId, missionType);
                int artistPointsBefore = DbContext.Artists.Single().Points;
                int reviewerPointsBefore = DbContext.Reviewers.Single().Points;

                for (int i = 0; i < missionType.GetMaxProgressCount(); i++)
                {
                        await missionManager.RecordProgressAsync(missionType, userId, date);
                }

                int artistPointsAfter = DbContext.Artists.Single().Points;
                int reviewerPointsAfter = DbContext.Reviewers.Single().Points;
                (artistPointsAfter - artistPointsBefore).Should().Be(expectedArtistPointsIncrease);
                (reviewerPointsAfter - reviewerPointsBefore).Should().Be(expectedReviewerPointsIncrease);
        }

        [Theory]
        [InlineData(MissionType.UploadArt, 25, 0)]
        [InlineData(MissionType.BoostArt, 25, 0)]
        [InlineData(MissionType.ReviewArt, 0, 25)]
        [InlineData(MissionType.LikeArt, 0, 25)]
        [InlineData(MissionType.VisitArtistsProfiles, 25, 25)]
        [InlineData(MissionType.VisitReviewersProfiles, 25, 25)]
        public async Task RecordProgress_ShouldNotAwardExtraPoints_ForAnAlreadyCompletedMission(
                MissionType missionType,
                int expectedArtistPointsIncrease,
                int expectedReviewerPointsIncrease)
        {
                await CreateUserWithArtistProfile();
                Guid userId = DbContext.Users.Single().Id;
                DateTimeOffset date = GetDateForUserThatGeneratesThisMission(userId, missionType);
                int artistPointsBefore = DbContext.Artists.Single().Points;
                int reviewerPointsBefore = DbContext.Reviewers.Single().Points;

                for (int i = 0; i < 2 * missionType.GetMaxProgressCount(); i++)
                {
                        await missionManager.RecordProgressAsync(missionType, userId, date);
                }

                int artistPointsAfter = DbContext.Artists.Single().Points;
                int reviewerPointsAfter = DbContext.Reviewers.Single().Points;
                (artistPointsAfter - artistPointsBefore).Should().Be(expectedArtistPointsIncrease);
                (reviewerPointsAfter - reviewerPointsBefore).Should().Be(expectedReviewerPointsIncrease);
        }

        [Theory]
        [InlineData(MissionType.UploadArt, 25, 0)]
        [InlineData(MissionType.BoostArt, 25, 0)]
        [InlineData(MissionType.ReviewArt, 0, 25)]
        [InlineData(MissionType.LikeArt, 0, 25)]
        [InlineData(MissionType.VisitArtistsProfiles, 25, 25)]
        [InlineData(MissionType.VisitReviewersProfiles, 25, 25)]
        public async Task RecordProgress_ShouldAwardPoints_WhenAnotherMissionCompletedYesterday(
                MissionType missionType,
                int expectedArtistPointsIncrease,
                int expectedReviewerPointsIncrease)
        {
                MissionGenerator missionGenerator = new();
                await CreateUserWithArtistProfile();
                Guid userId = DbContext.Users.Single().Id;
                DateTimeOffset date = GetDateForUserThatGeneratesThisMission(userId, missionType);
                DateTimeOffset yesterdaysDate = date.Subtract(TimeSpan.FromDays(1));
                MissionType yesterdaysMissionType = missionGenerator.GetMissions(userId, yesterdaysDate).Single();

                for (int i = 0; i < yesterdaysMissionType.GetMaxProgressCount(); i++)
                {
                        await missionManager.RecordProgressAsync(
                                yesterdaysMissionType,
                                userId,
                                yesterdaysDate);
                }

                int artistPointsBefore = DbContext.Artists.Single().Points;
                int reviewerPointsBefore = DbContext.Reviewers.Single().Points;

                for (int i = 0; i < missionType.GetMaxProgressCount(); i++)
                {
                        await missionManager.RecordProgressAsync(missionType, userId, date);
                }

                int artistPointsAfter = DbContext.Artists.Single().Points;
                int reviewerPointsAfter = DbContext.Reviewers.Single().Points;
                (artistPointsAfter - artistPointsBefore).Should().Be(expectedArtistPointsIncrease);
                (reviewerPointsAfter - reviewerPointsBefore).Should().Be(expectedReviewerPointsIncrease);
        }

        public static DateTimeOffset GetDateForUserThatGeneratesThisMission(Guid userId, MissionType missionType)
        {
                const int ITERATION_LIMIT = 1000;
                MissionGenerator missionGenerator = new();
                DateTimeOffset date = new(1970, 1, 1, 1, 1, 1, TimeSpan.Zero);

                for (int i = 0; i < ITERATION_LIMIT; i++)
                {
                        date = date.AddDays(1);
                        MissionType[] missions = missionGenerator.GetMissions(userId, date);

                        if (missions.Contains(missionType))
                        {
                                return date;
                        }
                }

                throw new InvalidOperationException($"Iteration limit of {ITERATION_LIMIT} exceeded: could not find a day that has mission '{missionType}' for user with id '{userId}'.");
        }

}
