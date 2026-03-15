using System;
using UnityEngine;
using System.Collections.Generic;

public class LeagueManager : Singleton<LeagueManager>
{
    private ILeagueRankingCalculator _rankingCalculator;
    private ILeaguePairingGenerator _swissPairingGenerator;
    private ILeaguePairingGenerator _tournamentPairingGenerator;
    private LeagueSaveData _currentLeague;

    protected override void Awake()
    {
        base.Awake();
        _rankingCalculator = new LeagueRankingCalculator();
        _swissPairingGenerator = new SwissPairingGenerator();
        _tournamentPairingGenerator = new TournamentPairingGenerator();
    }

    public LeagueSaveData CurrentLeague => _currentLeague;

    public void StartLeague(LeagueSaveData saveData)
    {
        if (saveData == null) return;

        _currentLeague = saveData;
        GenerateCurrentRoundMatchesIfNeeded();
        SaveCurrentLeague();
    }

    public void LoadLeague(string leagueId)
    {
        _currentLeague = LeagueDataManager.Instance.LoadLeague(leagueId);
    }

    public void CompleteMatch(LeagueMatchRecord match, int homeScore, int awayScore, string specialNote = "")
    {
        if (_currentLeague == null || match == null) return;
        if (_currentLeague.isFinished) return;

        if (IsTournament() && homeScore == awayScore)
        {
            Debug.LogError("토너먼트에서는 무승부가 허용되지 않습니다.");
            return;
        }

        match.isPlayed = true;
        match.homeScore = homeScore;
        match.awayScore = awayScore;
        match.specialNote = specialNote;

        if (IsTournament())
        {
            ApplyTournamentElimination(match);
        }

        RecalculateStandings();
        SaveCurrentLeague();
    }

    private void ApplyTournamentElimination(LeagueMatchRecord match)
    {
        string loserTeamId = match.homeScore > match.awayScore
            ? match.awayTeamId
            : match.homeTeamId;

        var loserEntry = _currentLeague.teams.Find(t => t.teamId == loserTeamId);
        if (loserEntry != null)
        {
            loserEntry.isEliminated = true;
        }

        if (loserTeamId == "Player_Team")
        {
            _currentLeague.isPlayerEliminated = true;
        }
    }

    public void EndRound()
    {
        if (_currentLeague == null) return;
        if (!IsCurrentRoundFinished()) return;

        RecalculateStandings();

        if (IsTournament())
        {
            if (CheckTournamentFinished())
            {
                FinishLeague();
                return;
            }
        }
        else
        {
            if (CheckSwissFinished())
            {
                FinishLeague();
                return;
            }
        }

        _currentLeague.currentRoundIndex++;
        GenerateCurrentRoundMatchesIfNeeded();
        SaveCurrentLeague();
    }

    private bool CheckTournamentFinished()
    {
        if (_currentLeague == null) return true;

        int aliveCount = 0;

        foreach (var team in _currentLeague.teams)
        {
            if (!team.isEliminated)
                aliveCount++;
        }

        return aliveCount <= 1;
    }

    private bool CheckSwissFinished()
    {
        if (_currentLeague == null) return true;

        var masterData = LeagueDataManager.Instance.GetMasterDataById(_currentLeague.leagueId);
        if (masterData == null) return true;

        return _currentLeague.currentRoundIndex >= masterData.Value.roundCount - 1;
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

    private bool IsTournament()
    {
        if (_currentLeague == null) return false;
        return _currentLeague.leagueType == "Tournament";
    }

    private bool IsCurrentRoundFinished()
    {
        if (_currentLeague == null) return false;

        bool hasAnyMatch = false;

        foreach (var match in _currentLeague.matchRecords)
        {
            if (match.roundIndex != _currentLeague.currentRoundIndex)
                continue;

            hasAnyMatch = true;

            if (!match.isPlayed)
                return false;
        }

        return hasAnyMatch;
    }

    private void GenerateCurrentRoundMatchesIfNeeded()
    {
        if (_currentLeague == null) return;

        bool alreadyExists = _currentLeague.matchRecords.Exists(
            m => m.roundIndex == _currentLeague.currentRoundIndex);

        if (alreadyExists) return;

        List<LeagueMatchRecord> matches = null;

        if (IsTournament())
        {
            matches = _tournamentPairingGenerator.GenerateRoundMatches(_currentLeague, _currentLeague.currentRoundIndex);

        }
        else
        {
            matches = _swissPairingGenerator.GenerateRoundMatches(_currentLeague, _currentLeague.currentRoundIndex);
        }

        if (matches == null || matches.Count == 0) return;

        _currentLeague.matchRecords.AddRange(matches);
    }
}