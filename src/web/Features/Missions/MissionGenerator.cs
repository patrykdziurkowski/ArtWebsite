namespace web.Features.Missions;

public class MissionGenerator
{
        /// <summary>
        /// This uses a seeded random number generator based off of input parameters to
        /// ensure that the same results are generated for each user for a given day.
        /// </summary>
        public MissionType[] GetMissions(Guid userId, DateTimeOffset now, int count = 1)
        {
                List<MissionType> possibleMissionTypes = [.. Enum.GetValues<MissionType>().Except([MissionType.Unknown])];
                if (count > possibleMissionTypes.Count)
                {
                        throw new ArgumentException($"Attempted to generate {count} missions when only {possibleMissionTypes.Count} are defined.");
                }

                int seed = userId.GetHashCode() ^ now.Date.GetHashCode();
                Random random = new(seed);

                MissionType[] missions = new MissionType[count];
                for (int i = 0; i < count; i++)
                {
                        MissionType randomRemainingMission = possibleMissionTypes[random.Next(0, possibleMissionTypes.Count)];
                        missions[i] = randomRemainingMission;
                        possibleMissionTypes.Remove(randomRemainingMission);
                }
                return missions;
        }
}
