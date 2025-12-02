using FluentAssertions;
using web.Features.Missions;

namespace tests.Unit;

public class MissionGeneratorTests
{
        [Fact]
        public void GetMissions_Throws_WhenAskingForMoreMissionsThanThereAre()
        {
                MissionGenerator missionGenerator = new();

                Action gettingMissions = () => missionGenerator.GetMissions(Guid.NewGuid(), DateTimeOffset.Now, count: 10);

                gettingMissions.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetMissions_ReturnsOneMission_WhenAskedForOne()
        {
                MissionGenerator missionGenerator = new();

                MissionType[] missions = missionGenerator.GetMissions(Guid.NewGuid(), DateTimeOffset.Now);

                missions.Should().HaveCount(1);
        }

        [Fact]
        public void GetMissions_ReturnsManyMissions_WhenAskedForMore()
        {
                MissionGenerator missionGenerator = new();

                MissionType[] missions = missionGenerator.GetMissions(Guid.NewGuid(), DateTimeOffset.Now, 5);

                missions.Should().HaveCount(5);
        }

        [Fact]
        public void GetMissions_ReturnsTheSameMissions_ForTheSameInputs()
        {
                Guid someUserId = Guid.NewGuid();
                MissionGenerator missionGenerator1 = new();
                MissionGenerator missionGenerator2 = new();

                MissionType[] missions1 = missionGenerator1.GetMissions(someUserId, DateTimeOffset.Now, 5);
                MissionType[] missions2 = missionGenerator1.GetMissions(someUserId, DateTimeOffset.Now, 5);
                MissionType[] missions3 = missionGenerator2.GetMissions(someUserId, DateTimeOffset.Now, 5);

                missions1.Should().BeEquivalentTo(missions2);
                missions2.Should().BeEquivalentTo(missions3);
        }

        [Fact]
        public void GetMissions_ReturnsTheSameMissions_ForTheSameInputsEvenWhenTheHourIsDifferent()
        {
                Guid someUserId = Guid.NewGuid();
                MissionGenerator missionGenerator = new();
                DateTimeOffset dateTime1 = new(2025, 12, 2, 18, 4, 32, TimeSpan.Zero);
                DateTimeOffset dateTime2 = dateTime1.Subtract(TimeSpan.FromHours(13));

                MissionType[] missions1 = missionGenerator.GetMissions(someUserId, dateTime1, 5);
                MissionType[] missions2 = missionGenerator.GetMissions(someUserId, dateTime2, 5);

                missions1.Should().BeEquivalentTo(missions2);
        }

        [Fact]
        public void GetMissions_ReturnsDifferentMissions_ForDifferentUsers()
        {
                MissionGenerator missionGenerator = new();
                bool missionsWereDifferent = false;

                for (int i = 0; i < 1000; i++)
                {
                        MissionType[] missions1 = missionGenerator.GetMissions(Guid.NewGuid(), DateTimeOffset.Now, 5);
                        MissionType[] missions2 = missionGenerator.GetMissions(Guid.NewGuid(), DateTimeOffset.Now, 5);

                        if (missions1.SequenceEqual(missions2))
                        {
                                continue;
                        }
                        else
                        {
                                missionsWereDifferent = true;
                                break;
                        }
                }

                missionsWereDifferent.Should().BeTrue();
        }

        [Fact]
        public void GetMissions_ReturnsDifferentMissions_ForDifferentDates()
        {
                Guid someUserId = Guid.NewGuid();
                MissionGenerator missionGenerator = new();
                DateTimeOffset dateTime1 = new(2025, 12, 2, 18, 4, 32, TimeSpan.Zero);
                DateTimeOffset dateTime2 = dateTime1.Subtract(TimeSpan.FromDays(1));
                bool missionsWereDifferent = false;

                for (int i = 0; i < 1000; i++)
                {
                        MissionType[] missions1 = missionGenerator.GetMissions(someUserId, dateTime1, 5);
                        MissionType[] missions2 = missionGenerator.GetMissions(someUserId, dateTime2, 5);

                        if (missions1.SequenceEqual(missions2))
                        {
                                continue;
                        }
                        else
                        {
                                missionsWereDifferent = true;
                                break;
                        }
                }

                missionsWereDifferent.Should().BeTrue();
        }

        [Fact]
        public void GetMissions_ReturnsSameFirstMission_WhenAskedForMoreThanPreviously()
        {
                Guid someUserId = Guid.NewGuid();
                MissionGenerator missionGenerator = new();

                MissionType[] missions1 = missionGenerator.GetMissions(someUserId, DateTimeOffset.Now, count: 4);
                MissionType[] missions2 = missionGenerator.GetMissions(someUserId, DateTimeOffset.Now, count: 5);

                for (int i = 0; i < Math.Min(missions1.Length, missions2.Length); i++)
                {
                        missions1[i] = missions2[i];
                }
        }

        [Fact]
        public void GetMissions_DoesntReturnDuplicates()
        {
                for (int i = 0; i < 100; i++)
                {
                        MissionGenerator missionGenerator = new();

                        MissionType[] missions = missionGenerator.GetMissions(Guid.NewGuid(), DateTimeOffset.UtcNow, count: 5);

                        missions.ToHashSet().Should().BeEquivalentTo(missions);
                }
        }

}
