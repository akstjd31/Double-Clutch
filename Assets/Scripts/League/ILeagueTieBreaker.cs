using UnityEngine;
/// <summary>
/// 타이 브레이커 규칙
/// </summary>
public interface ILeagueTieBreaker
{
    int Compare(LeagueStandingData a, LeagueStandingData b);
}

// 승점 우선
// public class PointsTieBreaker : ILeagueTieBreaker
// {
//     public int Compare(LeagueStandingData a, LeagueStandingData b)
//     {
//         return b.points.CompareTo(a.points);
//     }
// }

// 2. 득실차 우선
public class GoalDiffTieBreaker : ILeagueTieBreaker
{
    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        return b.goalDiff.CompareTo(a.goalDiff);
    }
}

// 다득점 우선
// public class ScoredTieBreaker : ILeagueTieBreaker
// {
//     public int Compare(LeagueStandingData a, LeagueStandingData b)
//     {
//         return b.scored.CompareTo(a.scored);
//     }
// }

// 1. 승수 우선
public class WinCountTieBreaker : ILeagueTieBreaker
{
    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        return b.win.CompareTo(a.win);
    }
}

public class TeamTierTieBreaker : ILeagueTieBreaker
{
    private string _leagueLevelId;

    public TeamTierTieBreaker(string leagueLevelId)
    {
        _leagueLevelId = leagueLevelId;
    }

    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        teamTier tierA = GetTier(a.teamId);
        teamTier tierB = GetTier(b.teamId);
        // enum 값이 클수록 상위이므로 내림차순 비교
        int result = tierB.CompareTo(tierA);

        // 티어가 완전히 동일할 때(또는 CSV 값을 못 불러와 둘 다 None일 때),
        // 알파벳 정렬로 인해 플레이어가 1위가 되는 것을 막고 기획 의도대로 하위로 밀어줍니다.
        if (result == 0)
        {
            if (a.teamId == StudentManager.TEAM_ID && b.teamId != StudentManager.TEAM_ID) return 1;
            if (b.teamId == StudentManager.TEAM_ID && a.teamId != StudentManager.TEAM_ID) return -1;
        }

        return result;
    }

    private teamTier GetTier(string teamId)
    {
        if (teamId == StudentManager.TEAM_ID)
        {
            var levelData = LeagueDataManager.Instance.GetLeagueLevelDataById(_leagueLevelId);
            if (levelData.HasValue)
            {
                return levelData.Value.playerTeamTier;
            }
            else
            {
                // [에러 로그] 플레이어 팀의 티어 데이터를 불러오지 못했을 때
                Debug.LogError($"[TeamTierTieBreaker] 플레이어 팀의 티어 데이터를 불러오지 못했습니다!\n" +
                               $"요청된 리그 레벨 ID: '{_leagueLevelId}'\n" +
                               $"League_Level_Table.csv 에 해당 ID가 정확히 일치하는지(띄어쓰기 등) 확인해주세요.");
                return teamTier.None;
            }
        }
        else
        {
            var rivalData = LeagueDataManager.Instance.GetRivalMasterDataById(teamId);
            if (rivalData.HasValue)
            {
                return rivalData.Value.teamTier;
            }
            else
            {
                // [에러 로그] 라이벌 팀의 데이터를 불러오지 못했을 때
                Debug.LogError($"[TeamTierTieBreaker] 라이벌 팀의 티어 데이터를 불러오지 못했습니다!\n" +
                               $"요청된 팀 ID: '{teamId}'\n" +
                               $"Rival_Master_Table.csv 에 해당 팀 ID가 정확히 일치하는지 확인해주세요.");
                return teamTier.None;
            }
        }
    }
}