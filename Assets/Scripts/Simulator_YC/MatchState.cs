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
    private bool _isHalfTimeEventTriggered = false;

    private bool _isBallInAir = false;

    public bool IsBallInAir
    {
        get => _isBallInAir;
        set => _isBallInAir = value;
    }

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
        _isHalfTimeEventTriggered = false;
        _isBallInAir = false;
    }

    //  하프타임 이벤트 효과 적용 함수 (껍데기)
    public void ApplyHalfTimeEffect(int choiceIndex)
    {
        // 기획서에 구체적인 수치 테이블이 없으므로 로그만 남김.
        // 추후 기획팀에서 [선택지별 효과 테이블]을 전달주면 여기에 스탯/컨디션 변경 코드 작성 필요.

        AddLog($"=== Half-Time Event Applied. Choice Index: {choiceIndex} ===");
    }
    public void SetReplayState(int quarter, float time)
    {
        _currentQuarter = quarter;
        _remainTime = time;
    }
    /// <summary>
    /// 시간 흐름 처리 (행동 하나당 소요 시간 차감)
    /// </summary>
    public void DecreaseTime(float seconds)
    {
        _remainTime -= seconds;

        // 시간이 0 이하가 됐을 때
        if (_remainTime <= 0)
        {
            _remainTime = 0; // 마이너스 시간 방지

            // [핵심] 공이 공중에 있다면(슛 하는 중) 아직 쿼터를 끝내지 않음!
            if (_isBallInAir == false)
            {
                EndQuarter();
            }
            else
            {
                AddLog("<color=yellow>Buzzer Beater Chance!</color>"); // 로그로 확인
            }
        }
    }
    // 슛이 끝났을 때(성공/실패 후) 호출할 함수
    public void CheckTimeOver()
    {
        // 시간이 다 됐는데 슛 때문에 기다리고 있었던 거라면 -> 이제 종료
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
