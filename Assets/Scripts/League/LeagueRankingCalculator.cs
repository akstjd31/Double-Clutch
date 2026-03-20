using System.Collections.Generic;
using System.Linq;

public interface ILeagueRankingCalculator
{
    List<LeagueStandingData> Calculate(LeagueSaveData saveData);
}

/// <summary>
/// 리그 순위 계산기 (5.2.2 참고)
/// </summary>
public class LeagueRankingCalculator : ILeagueRankingCalculator
{
    private readonly List<ILeagueTieBreaker> _tieBreakers;

    public LeagueRankingCalculator()
    {
        _tieBreakers = new List<ILeagueTieBreaker>
        {
            new WinCountTieBreaker(),
            // new PointsTieBreaker(),
            new GoalDiffTieBreaker()
            // new ScoredTieBreaker(),
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

        // 타이 브레이커 동적 생성
        var tieBreakers = new List<ILeagueTieBreaker>
    {
        new WinCountTieBreaker(), // 1순위: 승수
        new GoalDiffTieBreaker()  // 2순위: 득실차
    };

        // saveData.leagueId를 통해 현재 리그의 LevelId를 가져와 팀 티어 비교기 추가
        var masterData = LeagueDataManager.Instance.GetMasterDataById(saveData.leagueId);
        if (masterData.HasValue)
        {
            tieBreakers.Add(new TeamTierTieBreaker(masterData.Value.leagueLevelId));
        }

        // 타이 브레이커에 따른 정렬
        standings.Sort((a, b) =>
        {
            foreach (var tb in tieBreakers)
            {
                int result = tb.Compare(a, b);
                if (result != 0) return result;
            }
            // 끝까지 같으면 teamId 사전식 오름차순 정렬
            return string.Compare(a.teamId, b.teamId, System.StringComparison.Ordinal);
        });
        // 랭크 부여
        for (int i = 0; i < standings.Count; i++)
        {
            standings[i].rank = i + 1;
        }

        return standings;
    }
}