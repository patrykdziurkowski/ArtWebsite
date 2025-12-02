namespace web.Features.Missions;

public class MissionGenerator
{
        public MissionType[] GetMissions(Guid userId, DateTimeOffset now, int count = 1)
        {
                int seed = userId.GetHashCode() ^ now.Date.GetHashCode();
                Random random = new(seed);
                MissionType[] possibleMissionTypes = [.. Enum.GetValues<MissionType>().Except([MissionType.Unknown])];

                MissionType[] missions = new MissionType[count];
                for (int i = 0; i < count; i++)
                {
                        int nextMissionIndex = random.Next(0, possibleMissionTypes.Length);
                        missions[i] = possibleMissionTypes[nextMissionIndex];
                }
                return missions;
        }
}
