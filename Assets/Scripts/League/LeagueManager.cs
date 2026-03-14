public class LeagueManager
{
    private readonly ILeagueRankingCalculator _rankingCalculator;
    private readonly ILeagueSaveService _saveService;

    private LeagueRuntime _runtime;

    public LeagueManager(ILeagueRankingCalculator rankingCalculator, ILeagueSaveService saveService)
    {
        _rankingCalculator = rankingCalculator;
        _saveService = saveService;
    }

    public void StartLeague(LeagueSaveData saveData)
    {
        _runtime = new LeagueRuntime(saveData);
        _saveService.Save(_runtime.SaveData);
    }

    public void CompleteMatch(LeagueMatchRecord record, int homeScore, int awayScore, string specialNote = "")
    {
        record.isPlayed = true;
        record.homeScore = homeScore;
        record.awayScore = awayScore;
        record.specialNote = specialNote;

        RecalculateStandings();
        _saveService.Save(_runtime.SaveData);
    }

    public void EndRound()
    {
        _runtime.SaveData.currentRoundIndex++;
        RecalculateStandings();
        _saveService.Save(_runtime.SaveData);

        if (CheckLeagueFinished())
        {
            FinishLeague();
        }
    }

    public void OnPlayerEliminated()
    {
        _runtime.SaveData.isPlayerEliminated = true;

        //SimulateRemainingMatches();
        RecalculateStandings();
        FinishLeague();
    }

    // private void SimulateRemainingMatches()
    // {
    //     foreach (var match in _runtime.SaveData.matchRecords)
    //     {
    //         if (match.isPlayed) continue;

    //         var result = _simulator.Simulate(match.homeTeamId, match.awayTeamId);
    //         match.isPlayed = true;
    //         match.homeScore = result.HomeScore;
    //         match.awayScore = result.AwayScore;
    //         match.specialNote = "AUTO_SIMULATED";
    //     }
    // }

    private void RecalculateStandings()
    {
        _runtime.SaveData.standings = _rankingCalculator.Calculate(_runtime.SaveData);
    }

    private bool CheckLeagueFinished()
    {
        foreach (var match in _runtime.SaveData.matchRecords)
        {
            if (!match.isPlayed)
                return false;
        }

        return true;
    }

    private void FinishLeague()
    {
        _runtime.SaveData.isFinished = true;
        RecalculateStandings();
        _saveService.Save(_runtime.SaveData);
    }
}