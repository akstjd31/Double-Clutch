using System.Collections.Generic;
using UnityEngine;

public class MatchState : MonoBehaviour
{

    public const int MAX_QUARTER = 4;
    public const float SECONDS_PER_QUARTER = 600f; // 10분

    private int _currentQuarter;
    private float _remainTime; // 남은 시간 (초)
    private TeamSide _ballPossession; // 현재 공 소유 팀

    // 두 팀 정보
    private MatchTeam _homeTeam;
    private MatchTeam _awayTeam;

    // 경기 로그 (텍스트)
    private List<string> _matchLogs = new List<string>();

    public int CurrentQuarter => _currentQuarter;
    public float RemainTime => _remainTime;
    public TeamSide BallPossession => _ballPossession;
    public MatchTeam HomeTeam => _homeTeam;
    public MatchTeam AwayTeam => _awayTeam;
    private MatchUIManager _uiManager;


    private void Awake()
    {
        _uiManager = GetComponent<MatchUIManager>(); // 같은 오브젝트에 있다고 가정
    }

    /// <summary>
    /// 경기 초기화
    /// </summary>
    public void InitializeMatch(MatchTeam home, MatchTeam away)
    {
        _homeTeam = home;
        _awayTeam = away;
        _currentQuarter = 1;
        _remainTime = SECONDS_PER_QUARTER;
        _ballPossession = TeamSide.Home; // 점프볼 로직 전 임시
        _matchLogs.Clear();
    }

    /// <summary>
    /// 시간 흐름 처리 (행동 하나당 소요 시간 차감)
    /// </summary>
    public void DecreaseTime(float seconds)
    {
        _remainTime -= seconds;
        if (_remainTime <= 0)
        {
            EndQuarter();
        }
    }

    /// <summary>
    /// 공수 교대
    /// </summary>
    public void SwitchPossession()
    {
        _ballPossession = (_ballPossession == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;
    }

    /// <summary>
    /// 로그 기록 및 출력
    /// </summary>
    public void AddLog(string log)
    {
        _matchLogs.Add($"[{_currentQuarter}Q {_remainTime:F0}s] {log}");
        Debug.Log(_matchLogs[_matchLogs.Count - 1]);
        if (_uiManager != null)
            _uiManager.UpdateLogText(log);
    }

    private void EndQuarter()
    {
        AddLog($"--- {_currentQuarter} Quarter Ended ---");

        if (_currentQuarter < MAX_QUARTER)
        {
            _currentQuarter++;
            _remainTime = SECONDS_PER_QUARTER;
            // 쿼터 변경 시 필요한 로직 (공수 교대 등) 추가
        }
        else
        {
            EndMatch();
        }
    }

    private void EndMatch()
    {
        AddLog($"=== Match Ended. Final Score {_homeTeam.Score} : {_awayTeam.Score} ===");
        // 결과 저장 로직 호출
    }
}