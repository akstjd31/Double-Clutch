public class LeagueManager : Singleton<LeagueManager>
{
    private ILeagueRankingCalculator _rankingCalculator;
    private LeagueSaveData _currentLeague;

    protected override void Awake()
    {
        base.Awake();
        _rankingCalculator = new LeagueRankingCalculator();
    }

    public LeagueSaveData CurrentLeague => _currentLeague;

    public void StartLeague(LeagueSaveData saveData)
    {
        if (saveData == null) return;

        _currentLeague = saveData;
        SaveCurrentLeague();
    }

    public void LoadLeague(string leagueId)
    {
        _currentLeague = LeagueDataManager.Instance.LoadLeague(leagueId);
    }

    public void CompleteMatch(LeagueMatchRecord record, int homeScore, int awayScore, string specialNote = "")
    {
        if (_currentLeague == null) return;
        if (record == null) return;

        record.isPlayed = true;
        record.homeScore = homeScore;
        record.awayScore = awayScore;
        record.specialNote = specialNote;

        RecalculateStandings();
        SaveCurrentLeague();
    }

    public void EndRound()
    {
        if (_currentLeague == null) return;
        if (_currentLeague.isFinished) return;

        _currentLeague.currentRoundIndex++;

        RecalculateStandings();

        if (CheckLeagueFinished())
        {
            FinishLeague();
            return;
        }

        SaveCurrentLeague();
    }

    public void OnPlayerEliminated()
    {
        if (_currentLeague == null) return;
        if (_currentLeague.isFinished) return;

        _currentLeague.isPlayerEliminated = true;

        // 남은 경기 자동 시뮬레이션
        // SimulateRemainingMatches();

        RecalculateStandings();
        FinishLeague();
    }

    private void RecalculateStandings()
    {
        if (_currentLeague == null) return;

        _currentLeague.standings = _rankingCalculator.Calculate(_currentLeague);
    }

    private bool CheckLeagueFinished()
    {
        if (_currentLeague == null) return true;

        foreach (var match in _currentLeague.matchRecords)
        {
            if (!match.isPlayed)
                return false;
        }

        return true;
    }

    private void FinishLeague()
    {
        if (_currentLeague == null) return;

        _currentLeague.isFinished = true;
        RecalculateStandings();
        SaveCurrentLeague();
    }

    private void SaveCurrentLeague()
    {
        if (_currentLeague == null) return;

        LeagueDataManager.Instance.SaveLeague(_currentLeague);
    }
}