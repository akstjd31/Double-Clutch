using UnityEngine;
using Game.Constants;

/// <summary>
/// 타이 브레이커 규칙
/// 음수 : a가 b보다 앞
/// 양수 : b가 a보다 앞
/// 0    : 동순위
/// </summary>
public interface ILeagueTieBreaker
{
    int Compare(LeagueStandingData a, LeagueStandingData b);
}

// 1. 승수 우선
public class WinCountTieBreaker : ILeagueTieBreaker
{
    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        return b.win.CompareTo(a.win);
    }
}

// 2. 득실차 우선
public class GoalDiffTieBreaker : ILeagueTieBreaker
{
    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        return b.goalDiff.CompareTo(a.goalDiff);
    }
}

// 3. 팀 티어 우선
public class TeamTierTieBreaker : ILeagueTieBreaker
{
    private readonly string _leagueLevelId;

    public TeamTierTieBreaker(string leagueLevelId)
    {
        _leagueLevelId = leagueLevelId;
    }

    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        teamTier tierA = GetTier(a.teamId);
        teamTier tierB = GetTier(b.teamId);

        // enum 값이 클수록 상위 티어
        return tierB.CompareTo(tierA);
    }

    private teamTier GetTier(string teamId)
    {
        if (teamId == PrefKeys.PLAYER_TEAM_ID)
        {
            var levelData = LeagueDataManager.Instance.GetLeagueLevelDataById(_leagueLevelId);
            if (levelData.HasValue)
            {
                StudentManager.Instance.CurrentTeam.SetTier(levelData.Value.playerTeamTier);
                return levelData.Value.playerTeamTier;
            }

            Debug.LogError(
                $"[TeamTierTieBreaker] 플레이어 팀의 티어 데이터를 불러오지 못했습니다.\n" +
                $"leagueLevelId: {_leagueLevelId}");

            return teamTier.None;
        }

        var rivalData = LeagueDataManager.Instance.GetRivalMasterDataById(teamId);
        if (rivalData.HasValue)
            return rivalData.Value.teamTier;

        Debug.LogError(
            $"[TeamTierTieBreaker] 라이벌 팀의 티어 데이터를 불러오지 못했습니다.\n" +
            $"teamId: {teamId}");

        return teamTier.None;
    }
}

// 4. teamId 사전식 오름차순
public class TeamIdTieBreaker : ILeagueTieBreaker
{
    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        return string.Compare(a.teamId, b.teamId, System.StringComparison.Ordinal);
    }
}