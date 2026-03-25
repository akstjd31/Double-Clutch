using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 실제 리그 진행 처리
/// </summary>
public class LeagueManager : Singleton<LeagueManager>
{
    public const string PLAYER_TEAM_ID = "Player_Team";                // 플레이어 팀임을 구분짓는 스트링 키
    private LeagueDataManager _leagueDataMgr;
    private ILeagueRankingCalculator _rankingCalculator;            // 순위 계산
    private ILeaguePairingGenerator _swissPairingGenerator;         // 스위스
    private ILeaguePairingGenerator _tournamentPairingGenerator;    // 토너먼트
    [SerializeField] private LeagueSaveData _currentLeague;
    public LeagueSaveData CurrentLeague => _currentLeague;

    protected override void Awake()
    {
        base.Awake();
        _rankingCalculator = new LeagueRankingCalculator();
        _swissPairingGenerator = new SwissPairingGenerator();
        _tournamentPairingGenerator = new TournamentPairingGenerator();
        _leagueDataMgr = this.GetComponent<LeagueDataManager>();
    }

    private void Start()
    {
        LoadLeague();
    }

    // 리그 시작
    public void StartLeague(LeagueSaveData saveData)
    {
        if (saveData == null) return;

        _currentLeague = saveData;

        try
        {
            // 라운드 대진표 생성 전에 순위(랭킹)를 정확하게 계산합니다.
            if (_currentLeague.currentRoundIndex == 0 && _currentLeague.matchRecords.Count == 0)
            {
                RecalculateStandings();
            }
            // 1라운드(0 인덱스) 대진표 생성
            GenerateCurrentRoundMatchesIfNeeded();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[StartLeague] 대진표 생성 중 에러 발생: {e.Message}");
        }

        // 대진표까지 생성된 온전한 데이터를 최종 저장
        SaveCurrentLeague();
    }
    
    // 해당 리그 ID에 해당되는 데이터 캐싱 (전에 미리 데이터를 채워놔서 있다는 가정임)
    public void LoadLeague()
    {
        if (_leagueDataMgr == null) return;
        _currentLeague = _leagueDataMgr.LoadLeague();
        // 현재 리그가 진행 중일 때
        if (_currentLeague != null && !_currentLeague.isFinished)
        {
            // 대진표가 이미 잘 저장되어 있는지 확인
            bool alreadyExists = _currentLeague.matchRecords.Exists(
                m => m.roundIndex == _currentLeague.currentRoundIndex);

            // 대진표가 텅 비어있을 때만 복구 로직 실행
            if (!alreadyExists)
            {
                // 나중에 추적할 수 있도록 확실하게 경고 로그
                Debug.LogWarning($"<color=red>[LeagueManager]</color> 세이브 파일에 {_currentLeague.currentRoundIndex + 1}라운드 대진표가 없습니다! 자가 복구(재생성)를 시도합니다. (만약 새 게임인데도 이 로그가 뜬다면 저장 타이밍을 확인해야 합니다.)");

                GenerateCurrentRoundMatchesIfNeeded();
                SaveCurrentLeague(); // 복구한 김에 세이브 파일도 덮어씌움
            }
        }
    }

    public string GetOpponentTeamId(List<LeagueMatchRecord> records)
    {
        if (records == null) return null;
        foreach (var record in records)
        {
            if (record.homeTeamId.Equals(PLAYER_TEAM_ID))
                return record.awayTeamId;
        }

        return null;
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

        if (loserTeamId == PLAYER_TEAM_ID)
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

    // 토너먼트가 끝났는지? (남은 팀 수로 비교)
    private bool CheckTournamentFinished()
    {
        if (_currentLeague == null) return true;

        // 1팀 남으면 끝남 처리
        int aliveCount = 0;

        foreach (var team in _currentLeague.teams)
        {
            if (!team.isEliminated)
                aliveCount++;
        }

        return aliveCount <= 1;
    }

    // 스위스가 끝났는지? (라운드로 비교)
    private bool CheckSwissFinished()
    {
        if (_currentLeague == null) return true;

        var masterData = LeagueDataManager.Instance.GetMasterDataById(_currentLeague.leagueId);
        if (masterData == null) return true;

        return _currentLeague.currentRoundIndex >= masterData.Value.roundCount - 1;
    }

    // 우리 팀이 리그에서 탈락했을 경우 (시즌 아웃 확인 후 )
    public void OnPlayerEliminated()
    {
        if (_currentLeague == null) return;
        if (_currentLeague.isFinished) return;

        _currentLeague.isPlayerEliminated = true;

        // 남은 경기 자동 시뮬레이션
        // SimulateRemainingMatches();

        // RecalculateStandings();
        // FinishLeague();
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

        // 플레이어 팀이 우승했을 시 보상 처리
        var firstStanding = _currentLeague.standings[0];
        if (firstStanding == null)
        {
            Debug.LogError("스탠딩 데이터가 없음!");
            return;
        }
    }

    public bool IsPlayerSeasonOut()
    {
        if (_currentLeague == null) return false;

        var masterData = _leagueDataMgr.GetMasterDataById(_currentLeague.leagueId);
        if (masterData.Value.outConditionValue <= 0) return false;

        foreach (var standing in _currentLeague.standings)
        {
            // 플레이어의 순위와 비교
            if (standing.teamId.Equals(PLAYER_TEAM_ID))
            {
                // 순위에 들었는지 확인
                return standing.rank > masterData.Value.outConditionValue;
            }
        }

        return true;
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