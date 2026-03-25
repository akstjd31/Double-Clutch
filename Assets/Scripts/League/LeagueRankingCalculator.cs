using System;
using System.Collections.Generic;
using System.Linq;

public interface ILeagueRankingCalculator
{
    List<LeagueStandingData> Calculate(LeagueSaveData saveData);
}

/// <summary>
/// 리그 순위 계산기
/// 정렬 우선순위:
/// 1. 승수
/// 2. 득실차
/// 3. 팀 티어
/// 4. teamId 사전순
/// </summary>
public class LeagueRankingCalculator : ILeagueRankingCalculator
{
    public List<LeagueStandingData> Calculate(LeagueSaveData saveData)
    {
        var standings = BuildStandingList(saveData);
        if (standings.Count == 0)
            return standings;

        string leagueLevelId = GetLeagueLevelId(saveData);

        var tieBreakers = new List<ILeagueTieBreaker>
        {
            new WinCountTieBreaker(),
            new GoalDiffTieBreaker()
        };

        if (!string.IsNullOrEmpty(leagueLevelId))
        {
            tieBreakers.Add(new TeamTierTieBreaker(leagueLevelId));
        }

        tieBreakers.Add(new TeamIdTieBreaker());

        standings.Sort((a, b) =>
        {
            foreach (var tieBreaker in tieBreakers)
            {
                int result = tieBreaker.Compare(a, b);
                if (result != 0)
                    return result;
            }

            return string.Compare(a.teamId, b.teamId, StringComparison.Ordinal);
        });

        for (int i = 0; i < standings.Count; i++)
        {
            standings[i].rank = i + 1;
        }

        return standings;
    }

    private List<LeagueStandingData> BuildStandingList(LeagueSaveData saveData)
    {
        var result = new List<LeagueStandingData>();

        if (saveData == null || saveData.teams == null || saveData.teams.Count == 0)
            return result;

        var standingMap = new Dictionary<string, LeagueStandingData>();

        // 초기화
        foreach (var team in saveData.teams)
        {
            if (team == null || string.IsNullOrEmpty(team.teamId))
                continue;

            if (!standingMap.ContainsKey(team.teamId))
            {
                standingMap.Add(team.teamId, new LeagueStandingData
                {
                    teamId = team.teamId
                });
            }
        }

        if (saveData.matchRecords != null)
        {
            foreach (var match in saveData.matchRecords)
            {
                if (match == null || !match.isPlayed)
                    continue;

                if (!standingMap.ContainsKey(match.homeTeamId) || !standingMap.ContainsKey(match.awayTeamId))
                    continue;

                var home = standingMap[match.homeTeamId];
                var away = standingMap[match.awayTeamId];

                home.played++;
                away.played++;

                home.scored += match.homeScore;
                home.conceded += match.awayScore;

                away.scored += match.awayScore;
                away.conceded += match.homeScore;

                if (match.homeScore > match.awayScore)
                {
                    home.win++;
                    away.lose++;
                    home.points += 3;
                }
                else if (match.homeScore < match.awayScore)
                {
                    away.win++;
                    home.lose++;
                    away.points += 3;
                }
            }
        }

        foreach (var standing in standingMap.Values)
        {
            standing.goalDiff = standing.scored - standing.conceded;
        }

        result = standingMap.Values.ToList();
        return result;
    }

    private string GetLeagueLevelId(LeagueSaveData saveData)
    {
        if (saveData == null || string.IsNullOrEmpty(saveData.leagueId))
            return null;

        var masterData = LeagueDataManager.Instance.GetMasterDataById(saveData.leagueId);
        if (!masterData.HasValue)
            return null;

        return masterData.Value.leagueLevelId;
    }
}