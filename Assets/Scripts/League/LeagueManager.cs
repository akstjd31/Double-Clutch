using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 실제 리그 진행 처리
/// </summary>
public class LeagueManager : Singleton<LeagueManager>
{
    private ILeagueRankingCalculator _rankingCalculator;            // 순위 계산
    private ILeaguePairingGenerator _swissPairingGenerator;         // 스위스
    private ILeaguePairingGenerator _tournamentPairingGenerator;    // 토너먼트
    private LeagueSaveData _currentLeague;
    public LeagueSaveData CurrentLeague => _currentLeague;

    protected override void Awake()
    {
        base.Awake();
        _rankingCalculator = new LeagueRankingCalculator();
        _swissPairingGenerator = new SwissPairingGenerator();
        _tournamentPairingGenerator = new TournamentPairingGenerator();
    }

    // 리그 시작
    public void StartLeague(LeagueSaveData saveData)
    {
        if (saveData == null) return;

        _currentLeague = saveData;
        GenerateCurrentRoundMatchesIfNeeded();
        SaveCurrentLeague();
    }
    
    // 해당 리그 ID에 해당되는 데이터 캐싱 (전에 미리 데이터를 채워놔서 있다는 가정임)
    public void LoadLeague(string leagueId)
    {
        _currentLeague = LeagueDataManager.Instance.LoadLeague(leagueId);
    }

    // 경기 마무리에 따른 처리 (토너먼트 처리, 순위 갱신, 저장)
    public void CompleteMatch(LeagueMatchRecord match, int homeScore, int awayScore, string specialNote = "")
    {
        if (_currentLeague == null || match == null) return;
        if (_currentLeague.isFinished) return;

        // 무승부가 났을 떄??
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

    // 토너먼트 탈락 관련
    private void ApplyTournamentElimination(LeagueMatchRecord match)
    {
        // 탈락한 팀의 ID
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

    // 라운드가 종료되었을 떄
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

    // 토너먼트가 끝났는지?
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

    // 스위스가 끝났는지?
    private bool CheckSwissFinished()
    {
        if (_currentLeague == null) return true;

        var masterData = LeagueDataManager.Instance.GetMasterDataById(_currentLeague.leagueId);
        if (masterData == null) return true;

        return _currentLeague.currentRoundIndex >= masterData.Value.roundCount - 1;
    }

    // 우리 팀이 리그에서 탈락했을 경우
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

    // 라운드가 종료되었는지?
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

    // 현재 라운드의 경기 매칭을 아직 만들지 않았다면 리스트 생성
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