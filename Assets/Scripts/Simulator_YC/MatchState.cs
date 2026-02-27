using System.Collections.Generic;
using UnityEngine;

public class MatchState : MonoBehaviour
{

    [Header("Data Readers")]
    [SerializeField] private Halftime_ScriptDataReader _halftimeScriptReader;
    [SerializeField] private Halftime_ListDataReader _halftimeListReader;

    public string CurrentHalftimeScriptId;

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

    public Halftime_ScriptDataReader HalftimeScriptReader => _halftimeScriptReader;
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
    public void ApplyHalfTimeEffectDirectly(potential stat, float statAmount, Position targetPos, changeType posChange)
    {
        // 효과가 없는 선택지라면 바로 종료
        if (stat == potential.None && posChange == changeType.None) return;

        foreach (var player in _homeTeam.Roster)
        {
            // 대상 포지션이 일치하거나, 특정 포지션 지목이 없는 경우(None)에만 적용
            if (targetPos == Position.None || player.MainPosition == targetPos)
            {
                // 포지션(진형) 변경
                if (posChange != changeType.None)
                    player.TempPositionChange = posChange;

                // 스탯 버프 적용
                if (stat != potential.None)
                {
                    MatchStatType matchStat = ConvertPotentialToMatchStat(stat);
                    player.AddTempStatBuff(matchStat, statAmount);
                }
            }
        }

        AddLog($"<color=orange>=== 하프타임 전술 지시 적용 완료! ===</color>");
    }

    // 잠재력 Enum을 MatchStat Enum으로 변환해주는 헬퍼 함수
    private MatchStatType ConvertPotentialToMatchStat(potential p)
    {
        switch (p)
        {
            case potential.Stat2pt: return MatchStatType.TwoPoint;
            case potential.Stat3pt: return MatchStatType.ThreePoint;
            case potential.StatPass: return MatchStatType.Pass;
            case potential.StatBlock: return MatchStatType.Block;
            case potential.StatSteal: return MatchStatType.Steal;
            case potential.StatRebound: return MatchStatType.Rebound;
            default: return MatchStatType.TwoPoint;
        }
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
    // 전반전 종료 후, 현재 팀 상황에 맞는 하프타임 스크립트 ID를 결정하는 함수
    public void DetermineHalftimeEvent()
    {
        if (_halftimeListReader == null) return;

        List<Halftime_ListData> candidates = new List<Halftime_ListData>();

        foreach (var data in _halftimeListReader.DataList)
        {
            bool isConditionMet = false;

            switch (data.triggerCond)
            {
                case triggerCond.Random: // 언제든 등장 가능
                    isConditionMet = true;
                    break;
                case triggerCond.ScoreGap: // 점수가 지고 있을 때 (유저 점수 - 상대 점수 <= 기준값)
                    isConditionMet = (_homeTeam.Score - _awayTeam.Score) <= data.triggerValue;
                    break;
                case triggerCond.ReboundDiff: // 리바운드가 밀릴 때
                    isConditionMet = (_homeTeam.ReboundCount - _awayTeam.ReboundCount) <= data.triggerValue;
                    break;
                case triggerCond.Stat2ptLow: // 2점슛 확률이 낮을 때 (기준값 % 이하)
                    float pt2Rate = _homeTeam.Try2pt == 0 ? 0 : ((float)_homeTeam.Succ2pt / _homeTeam.Try2pt) * 100f;
                    isConditionMet = _homeTeam.Try2pt > 0 && pt2Rate <= data.triggerValue;
                    break;
                case triggerCond.Stat3ptLow: // 3점슛 확률이 낮을 때 (기준값 % 이하)
                    float pt3Rate = _homeTeam.Try3pt == 0 ? 0 : ((float)_homeTeam.Succ3pt / _homeTeam.Try3pt) * 100f;
                    isConditionMet = _homeTeam.Try3pt > 0 && pt3Rate <= data.triggerValue;
                    break;
            }

            if (isConditionMet) candidates.Add(data);
        }

        // 조건을 만족하는 이벤트들 중 하나를 랜덤으로 선택
        if (candidates.Count > 0)
        {
            int rnd = UnityEngine.Random.Range(0, candidates.Count);
            CurrentHalftimeScriptId = candidates[rnd].scriptId;
        }
        else
        {
            CurrentHalftimeScriptId = "Script_Halftime_001"; // 만족하는 게 없으면 기본 이벤트
        }

        AddLog($"[시스템] 하프타임 이벤트 결정됨 (Script ID: {CurrentHalftimeScriptId})");
    }
}
