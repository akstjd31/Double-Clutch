using System.Collections.Generic;
using System.Linq;

public class SwissPairingGenerator : ILeaguePairingGenerator
{
    public List<LeagueMatchRecord> GenerateRoundMatches(LeagueSaveData saveData, int roundIndex)
    {
        var result = new List<LeagueMatchRecord>();
        if (saveData == null || saveData.teams == null || saveData.teams.Count < 2)
            return result;

        List<string> sortedTeams;

        if (saveData.standings != null && saveData.standings.Count > 0)
        {
            sortedTeams = saveData.standings.OrderBy(s => s.rank).Select(s => s.teamId).ToList();
        }
        else
        {
            // 혹시라도 순위 데이터가 누락된 최악의 예외 상황에만 방어용으로 참가 명단 순서 사용
            sortedTeams = saveData.teams.Select(t => t.teamId).ToList();
        }

        // 위에서부터 순서대로 딱 2팀씩 끊어서 무조건 매칭 (1vs2, 3vs4, 5vs6...)
        for (int i = 0; i < sortedTeams.Count; i += 2)
        {
            if (i + 1 >= sortedTeams.Count) break; // 혹시 모를 에러 방지

            string teamA = sortedTeams[i];
            string teamB = sortedTeams[i + 1];

            // UI 표기와 편의성을 위해 플레이어 팀은 무조건 Home(왼쪽)으로 고정
            if (teamB == PrefKeys.PLAYER_TEAM_ID)
            {
                string temp = teamA;
                teamA = teamB;
                teamB = temp;
            }

            result.Add(new LeagueMatchRecord
            {
                roundIndex = roundIndex,
                homeTeamId = teamA,
                awayTeamId = teamB,
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