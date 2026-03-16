/// <summary>
/// 타이 브레이커 규칙
/// </summary>
public interface ILeagueTieBreaker
{
    int Compare(LeagueStandingData a, LeagueStandingData b);
}

// 승점 우선
public class PointsTieBreaker : ILeagueTieBreaker
{
    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        return b.points.CompareTo(a.points);
    }
}

// 득실차 우선
public class GoalDiffTieBreaker : ILeagueTieBreaker
{
    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        return b.goalDiff.CompareTo(a.goalDiff);
    }
}

// 다득점 우선
public class ScoredTieBreaker : ILeagueTieBreaker
{
    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        return b.scored.CompareTo(a.scored);
    }
}

// 승수 우선
public class WinCountTieBreaker : ILeagueTieBreaker
{
    public int Compare(LeagueStandingData a, LeagueStandingData b)
    {
        return b.win.CompareTo(a.win);
    }
}
