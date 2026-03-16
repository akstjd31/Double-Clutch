using System.Collections.Generic;
using System.Linq;

public class TournamentPairingGenerator : ILeaguePairingGenerator
{
    public List<LeagueMatchRecord> GenerateRoundMatches(LeagueSaveData saveData, int roundIndex)
    {
        var result = new List<LeagueMatchRecord>();
        if (saveData == null || saveData.teams == null)
            return result;

        var aliveTeams = GetAliveTeams(saveData);

        if (aliveTeams.Count < 2)
            return result;

        // 첫 라운드는 초기 시드 순서 / 이후 라운드는 생존 팀 순서
        for (int i = 0; i < aliveTeams.Count; i += 2)
        {
            if (i + 1 >= aliveTeams.Count)
                break;

            result.Add(new LeagueMatchRecord
            {
                roundIndex = roundIndex,
                homeTeamId = aliveTeams[i],
                awayTeamId = aliveTeams[i + 1],
                isPlayed = false,
                homeScore = 0,
                awayScore = 0,
                specialNote = string.Empty,
                replayLogKey = string.Empty
            });
        }

        return result;
    }

    private List<string> GetAliveTeams(LeagueSaveData saveData)
    {
        return saveData.teams
            .Where(t => !t.isEliminated)
            .Select(t => t.teamId)
            .ToList();
    }
}