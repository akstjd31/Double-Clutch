using System.Collections.Generic;
using System.Linq;

public interface ILeagueRankingCalculator
{
    List<LeagueStandingData> Calculate(LeagueSaveData saveData);
}

/// <summary>
/// 리그 순위 계산기
/// </summary>
public class LeagueRankingCalculator : ILeagueRankingCalculator
{
    private readonly List<ILeagueTieBreaker> _tieBreakers;

    public LeagueRankingCalculator()
    {
        _tieBreakers = new List<ILeagueTieBreaker>
        {
            new PointsTieBreaker(),
            new GoalDiffTieBreaker(),
            new ScoredTieBreaker(),
            new WinCountTieBreaker()
        };
    }

    public List<LeagueStandingData> Calculate(LeagueSaveData saveData)
    {
        var standingMap = new Dictionary<string, LeagueStandingData>();

        // 초기화
        foreach (var team in saveData.teams)
        {
            standingMap[team.teamId] = new LeagueStandingData
            {
                teamId = team.teamId
            };
        }

        // 경기 결과 누적
        foreach (var match in saveData.matchRecords)
        {
            if (!match.isPlayed) continue;

            var home = standingMap[match.homeTeamId];
            var away = standingMap[match.awayTeamId];

            home.played++;
            away.played++;

            home.scored += match.homeScore;
            home.conceded += match.awayScore;

            away.scored += match.awayScore;
            away.conceded += match.homeScore;

            // 홈팀 승
            if (match.homeScore > match.awayScore)
            {
                home.win++;
                away.lose++;
                home.points += 3;
            }

            // 어웨이팀 승
            else if (match.homeScore < match.awayScore)
            {
                away.win++;
                home.lose++;
                away.points += 3;
            }
            // 무승부는 없기떄문에 일단 뻄
            // else
            // {
            //     home.points += 1;
            //     away.points += 1;
            // }
        }

        foreach (var standing in standingMap.Values)
        {
            standing.goalDiff = standing.scored - standing.conceded;
        }

        var standings = standingMap.Values.ToList();

        standings.Sort(CompareTeams);

        for (int i = 0; i < standings.Count; i++)
        {
            standings[i].rank = i + 1;
        }

        return standings;
    }

    private int CompareTeams(LeagueStandingData a, LeagueStandingData b)
    {
        foreach (var tieBreaker in _tieBreakers)
        {
            int result = tieBreaker.Compare(a, b);
            if (result != 0)
                return result;
        }

        // 끝까지 같으면 teamId로 고정 정렬
        return string.Compare(a.teamId, b.teamId, System.StringComparison.Ordinal);
    }
}