using System.Collections.Generic;
using System.Linq;

public class TournamentPairingGenerator : ILeaguePairingGenerator
{
    public List<LeagueMatchRecord> GenerateRoundMatches(LeagueSaveData saveData, int roundIndex)
    {
        var result = new List<LeagueMatchRecord>();
        if (saveData == null || saveData.teams == null)
            return result;

        // 탈락하지 않은(isEliminated == false) 생존 팀만 차례대로 추출
        var aliveTeams = saveData.teams
            .Where(t => !t.isEliminated)
            .Select(t => t.teamId)
            .ToList();

        if (aliveTeams.Count < 2)
            return result;

        // 생존한 팀들을 브라켓 순서대로 2팀씩 매칭
        for (int i = 0; i < aliveTeams.Count; i += 2)
        {
            if (i + 1 >= aliveTeams.Count) break;

            string homeId = aliveTeams[i];
            string awayId = aliveTeams[i + 1];

            if (awayId == PrefKeys.PLAYER_TEAM_ID)
            {
                string temp = homeId;
                homeId = awayId;
                awayId = temp;
            }

            result.Add(new LeagueMatchRecord
            {
                roundIndex = roundIndex,
                homeTeamId = homeId,
                awayTeamId = awayId,
                isPlayed = false,
                homeScore = 0,
                awayScore = 0,
                specialNote = string.Empty,
                replayLogKey = string.Empty
            });
        }

        return result;
    }
}