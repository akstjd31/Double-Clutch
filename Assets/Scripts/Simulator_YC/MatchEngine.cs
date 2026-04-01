using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchEngine : MonoBehaviour
{
    public List<MatchLogData> MatchLogs = new List<MatchLogData>();

    // 1쿼터부터 끝날때까지 절대 지워지지 않고 누적되는 전체 로그 보관용
    public List<MatchLogData> FullMatchLogs = new List<MatchLogData>();

    public Action OnMatchEnded;

    private float _simTime;
    private int _simQuarter;
    private MatchTeam _homeTeam;
    private MatchTeam _awayTeam;
    private TeamSide _currentPossession;
    private MatchPlayer _ballHolder;
    private const float MAX_MOVE_PER_TICK = 1f / 3f; // 기획서 5.3: 틱당 최대 이동거리
    private bool _isTransitionTurn = false;
    private bool _isInboundTurn = false;

    [Header("Data Readers")]
    [SerializeField] private Event_ConfigDataReader _eventConfigReader;
    [SerializeField] private Position_PresetDataReader _positionPresetReader;
    [SerializeField] private Player_SynergyDataReader _synergyReader;

    [Header("Balance Settings")]
    [SerializeField]
    [Tooltip("드리블 시 수비수에게 방해받는 판정 거리")]
    private float dribbleBlockDist = 0.1f;
    [SerializeField]
    [Tooltip("패스 시 수비수에게 차단당하는 판정 거리")]
    private float passInterceptDist = 0.03f;
    [SerializeField]
    [Tooltip("슛 시도 시 수비수에게 블록당하는 판정 거리")]
    private float blockDist = 0.25f;
    public void StartSimulation()
    {
        // 엔진 내부에서 코루틴을 돌려 전반전/하프타임/후반전 흐름을 제어합니다.
        StartCoroutine(MatchFlowRoutine());
    }

    private IEnumerator MatchFlowRoutine()
    {
        MatchState state = FindFirstObjectByType<MatchState>();
        MatchReplayer replayer = FindFirstObjectByType<MatchReplayer>();
        MatchUIManager uiManager = FindFirstObjectByType<MatchUIManager>();

        if (state == null || state.HomeTeam == null || state.AwayTeam == null)
        {
            Debug.LogError("[MatchEngine] MatchState 또는 팀 정보가 없습니다.");
            OnMatchEnded?.Invoke();
            yield break;
        }

        // 초기 세팅
        InitMatchData(state.HomeTeam, state.AwayTeam);

        // 어웨이 팀 좌표 반전 (1.0 기준 대칭)
        foreach (var player in state.AwayTeam.Roster)
        {
            if (player.LogicPosition.y >= 0.5f)
                player.LogicPosition = new Vector2(player.LogicPosition.x, 1.0f - player.LogicPosition.y);
        }

        // 전반전(1~2쿼터) 연산
        CalculateUntilQuarter(2);

        // 전반전이 끝났으므로 스탯을 평가하여 하프타임 이벤트를 정합니다.
        state.DetermineHalftimeEvent();

        // 전반전 재생 시작
        bool isReplayDone = false;
        if (replayer != null)
        {
            replayer.Init(MatchLogs);
            replayer.OnReplayEnded = () => { isReplayDone = true; };
            replayer.PlayMatch();

            // 화면 재생이 끝날 때까지 엔진 대기
            yield return new WaitUntil(() => isReplayDone);
        }

        if (uiManager != null)
        {
            uiManager.ShowQuarterEndPopup();
            yield return new WaitUntil(() => uiManager.IsQuarterEndConfirmed);
        }

        // 하프타임 이벤트 패널 대기
        if (uiManager != null)
        {
            // 스크립트 ID를 넘겨주며 비주얼 노벨 시작
            uiManager.StartHalftimeEvent(state.CurrentHalftimeScriptId);

            // 유저가 대사를 모두 읽고 최종 End를 누를 때까지(IsEventFinished == true) 무한 대기
            yield return new WaitUntil(() => uiManager.IsEventFinished);
        }
        // 후반전 재생 시작 전에 CourtPanel 자식 전부 즉시 삭제
        foreach (Transform child in replayer.CourtPanel)
        {
            Destroy(child.gameObject);
        }

        // 후반전(3~4쿼터 및 연장전) 연산
        CalculateUntilQuarter(4);

        // 후반전 재생 시작
        isReplayDone = false;
        if (replayer != null)
        {
            replayer.Init(MatchLogs); // 후반전 로그 새로 세팅
            // OnReplayEnded는 전반전에서 연결한 무명함수가 그대로 작동함
            replayer.PlayMatch();

            // 후반전 재생 끝날 때까지 대기
            yield return new WaitUntil(() => isReplayDone);
        }

        // 경기 완전 종료 -> MatchSimState로 신호 전달!
        OnMatchEnded?.Invoke();
    }

    public void InitMatchData(MatchTeam home, MatchTeam away)
    {
        _homeTeam = home;
        _awayTeam = away;
        _simQuarter = 1;
        _simTime = 600f; // 10분
        _currentPossession = TeamSide.Home;
        _isTransitionTurn = false;
        FullMatchLogs.Clear(); // 새 경기 시작 시 전체 로그 초기화

        _homeTeam.SimulatedScore = 0;
        _awayTeam.SimulatedScore = 0;

        if (_synergyReader != null)
        {
            _homeTeam.EvaluateSynergies(_synergyReader.DataList);
            _awayTeam.EvaluateSynergies(_synergyReader.DataList);
        }
    }

    public void CalculateUntilQuarter(int targetQuarter)
    {
        MatchLogs.Clear();

        Debug.Log($">>> [MatchEngine] Simulation Phase: {_simQuarter}Q ~ {targetQuarter}Q");

        // 일반 쿼터 처리 (1~4쿼터)
        while (_simQuarter <= targetQuarter)
        {
            MatchTeam attackTeam = (_currentPossession == TeamSide.Home) ? _homeTeam : _awayTeam;
            _ballHolder = attackTeam.GetPlayerByPosition(Position.PG) ?? attackTeam.Roster[0];

            RecordLog("GameStart");
            while (_simTime > 0)
            {
                ProcessTurn();
            }

            RecordLog("QuarterEnd");

            _simQuarter++;
            _simTime = 600f;
            _currentPossession = (_currentPossession == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;
        }

        // 연장전 처리 (3쿼터가 끝났는데 동점일 때만)
        if (targetQuarter >= 4)
        {
            // 4쿼터 종료 직후 동점이 아니면 (연장전에 갈 필요가 없으면)
            if (_homeTeam.SimulatedScore != _awayTeam.SimulatedScore)
            {
                // 다음 쿼터를 위해 미리 올려둔 _simQuarter를 다시 원래 쿼터(4Q)로 되돌림
                _simQuarter--;
            }
            else
            {
                // 동점일 경우 연장전 돌입
                while (_homeTeam.SimulatedScore == _awayTeam.SimulatedScore)
                {
                    // 연장전 쿼터 시작 시에도 볼 핸들러 세팅 유지
                    MatchTeam attackTeam = (_currentPossession == TeamSide.Home) ? _homeTeam : _awayTeam;
                    _ballHolder = attackTeam.GetPlayerByPosition(Position.PG) ?? attackTeam.Roster[0];

                    RecordLog("GameStart");

                    _simTime = 300f;

                    while (_simTime > 0)
                    {
                        ProcessTurn();
                    }

                    RecordLog("QuarterEnd");

                    // 연장 쿼터 종료 직후 승부가 났으면, 
                    // 다음 쿼터로 올리지 않고 바로 루프 탈출 (그래야 GameEnd가 해당 연장 쿼터로 찍힘)
                    if (_homeTeam.SimulatedScore != _awayTeam.SimulatedScore)
                        break;

                    _simQuarter++;
                    _currentPossession = (_currentPossession == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;
                    if (_simQuarter > 7) break;
                }
            }

            RecordLog("GameEnd");
        }
    }

    private void ProcessTurn()
    {
        // [시너지용] 매 턴(틱)이 시작될 때마다 모든 선수의 패스 버프 지속시간을 1씩 깎음
        foreach (var p in _homeTeam.Roster) { if (p.PassReceivedBuffTick > 0) p.PassReceivedBuffTick--; }
        foreach (var p in _awayTeam.Roster) { if (p.PassReceivedBuffTick > 0) p.PassReceivedBuffTick--; }

        MatchTeam attackTeam = (_currentPossession == TeamSide.Home) ? _homeTeam : _awayTeam;
        MatchTeam defendTeam = (_currentPossession == TeamSide.Home) ? _awayTeam : _homeTeam;

        if (_ballHolder == null || !attackTeam.Roster.Contains(_ballHolder))
            _ballHolder = attackTeam.GetPlayerByPosition(Position.PG) ?? attackTeam.Roster[0];

        // 공수 전환 시 1틱의 시간을 소모하며 중간(0.45)까지만 이동하는 전용 턴
        if (_isTransitionTurn)
        {
            // 원래 틱과 완벽히 동일한 시간 소모
            float transTimeCost = UnityEngine.Random.Range(1f, 5f);
            _simTime -= transTimeCost;
            if (_simTime <= 0) _simTime = 0;

            if (_isInboundTurn)
            {
                // 골 먹힌 케이스: PG가 골대 밑에서 인바운드 패스 준비
                MatchPlayer inbounder = _ballHolder;
                MatchPlayer bestReceiver = null;
                float maxScore = -999f;
                foreach (var mate in attackTeam.Roster)
                {
                    if (mate == inbounder) continue;
                    float minEnemyDist = float.MaxValue;
                    foreach (var e in defendTeam.Roster)
                    {
                        float d = MatchCalculator.CalculateDistance(mate.LogicPosition, e.LogicPosition);
                        if (d < minEnemyDist) minEnemyDist = d;
                    }
                    if (minEnemyDist > maxScore) { maxScore = minEnemyDist; bestReceiver = mate; }
                }

                if (bestReceiver != null)
                {
                    // 1차 이동: Y축 0.1~0.2 전진 및 코트 범위(0.05 ~ 0.95) 이탈 방지
                    foreach (var p in attackTeam.Roster)
                    {
                        if (p == inbounder) continue;
                        Vector2 targetPos = GetPreferredPosition(p, true, attackTeam.Side);
                        float dirY = targetPos.y - p.LogicPosition.y;
                        float randMove = UnityEngine.Random.Range(0.1f, 0.2f);
                        p.LogicPosition = new Vector2(p.LogicPosition.x, Mathf.Clamp(p.LogicPosition.y + Mathf.Clamp(dirY, -randMove, randMove), 0.05f, 0.95f));
                    }
                    foreach (var p in defendTeam.Roster)
                    {
                        Vector2 targetPos = GetPreferredPosition(p, false, defendTeam.Side);
                        float dirY = targetPos.y - p.LogicPosition.y;
                        float randMove = UnityEngine.Random.Range(0.1f, 0.2f);
                        p.LogicPosition = new Vector2(p.LogicPosition.x, Mathf.Clamp(p.LogicPosition.y + Mathf.Clamp(dirY, -randMove, randMove), 0.05f, 0.95f));
                    }

                    // 1차 이동 로그 기록
                    MatchLogData step1Log = new MatchLogData();
                    step1Log.GameTime = Mathf.Max(0, _simTime);
                    step1Log.Quarter = _simQuarter;
                    step1Log.TeamId = (_currentPossession == TeamSide.Home) ? 0 : 1;
                    step1Log.LogText = "";
                    step1Log.EventType = "TRANSITION";
                    step1Log.IsCutIn = false;
                    step1Log.CutInType = "";
                    step1Log.SfxType = "";
                    step1Log.BallPos = inbounder.LogicPosition;
                    for (int i = 0; i < 5; i++)
                    {
                        if (_homeTeam.Roster.Count > i) step1Log.HomePositions[i] = _homeTeam.Roster[i].LogicPosition;
                        if (_awayTeam.Roster.Count > i) step1Log.AwayPositions[i] = _awayTeam.Roster[i].LogicPosition;
                    }
                    MatchLogs.Add(step1Log);
                    FullMatchLogs.Add(step1Log);

                    // 2차 이동: 패스 날아가면서 멈춰있지 않도록 직전에 미리 0.1~0.2 추가 이동
                    foreach (var p in attackTeam.Roster)
                    {
                        if (p == inbounder) continue;
                        Vector2 targetPos = GetPreferredPosition(p, true, attackTeam.Side);
                        float dirY = targetPos.y - p.LogicPosition.y;
                        float randMove = UnityEngine.Random.Range(0.1f, 0.2f);
                        p.LogicPosition = new Vector2(p.LogicPosition.x, Mathf.Clamp(p.LogicPosition.y + Mathf.Clamp(dirY, -randMove, randMove), 0.05f, 0.95f));
                    }
                    foreach (var p in defendTeam.Roster)
                    {
                        Vector2 targetPos = GetPreferredPosition(p, false, defendTeam.Side);
                        float dirY = targetPos.y - p.LogicPosition.y;
                        float randMove = UnityEngine.Random.Range(0.1f, 0.2f);
                        p.LogicPosition = new Vector2(p.LogicPosition.x, Mathf.Clamp(p.LogicPosition.y + Mathf.Clamp(dirY, -randMove, randMove), 0.05f, 0.95f));
                    }

                    _ballHolder = bestReceiver;
                    bestReceiver.PassReceivedBuffTick = 4;
                    RecordLog("PassSucc", inbounder, bestReceiver);
                }

                // 모든 연출이 끝났으므로 다음 일반 턴을 위해 제한 해제
                _isTransitionTurn = false;
                _isInboundTurn = false;
                return;
            }
            else
            {
                // 스틸/리바운드 케이스: 볼홀더 그 자리에서 바로 패스, 전원 이동
                MatchPlayer passer = _ballHolder;
                MatchPlayer bestReceiver = null;
                float maxScore = -999f;
                foreach (var mate in attackTeam.Roster)
                {
                    if (mate == passer) continue;
                    float minEnemyDist = float.MaxValue;
                    foreach (var e in defendTeam.Roster)
                    {
                        float d = MatchCalculator.CalculateDistance(mate.LogicPosition, e.LogicPosition);
                        if (d < minEnemyDist) minEnemyDist = d;
                    }
                    if (minEnemyDist > maxScore) { maxScore = minEnemyDist; bestReceiver = mate; }
                }
                if (bestReceiver != null)
                {
                    _ballHolder = bestReceiver;
                }
                // 전원 이동 (볼홀더=bestReceiver 포함, passer 포함)
                foreach (var p in attackTeam.Roster)
                {
                    if (p == passer) continue;
                    Vector2 targetPos = GetPreferredPosition(p, true, attackTeam.Side);
                    float dirY = targetPos.y - p.LogicPosition.y;
                    p.LogicPosition = new Vector2(p.LogicPosition.x, Mathf.Clamp01(p.LogicPosition.y + Mathf.Clamp(dirY, -0.45f, 0.45f)));
                }
                foreach (var p in defendTeam.Roster)
                {
                    Vector2 targetPos = GetPreferredPosition(p, false, defendTeam.Side);
                    float dirY = targetPos.y - p.LogicPosition.y;
                    p.LogicPosition = new Vector2(p.LogicPosition.x, Mathf.Clamp01(p.LogicPosition.y + Mathf.Clamp(dirY, -0.45f, 0.45f)));
                }

                if (bestReceiver != null)
                {
                    RecordLog("PassSucc", passer, bestReceiver);
                }
            }

            MatchLogData transLog = new MatchLogData();
            transLog.GameTime = Mathf.Max(0, _simTime);
            transLog.Quarter = _simQuarter;
            transLog.TeamId = (_currentPossession == TeamSide.Home) ? 0 : 1;
            transLog.LogText = "";
            transLog.EventType = "TRANSITION";
            transLog.IsCutIn = false;
            transLog.CutInType = "";
            transLog.SfxType = "";
            if (_ballHolder != null) transLog.BallPos = _ballHolder.LogicPosition;
            for (int i = 0; i < 5; i++)
            {
                if (_homeTeam.Roster.Count > i) transLog.HomePositions[i] = _homeTeam.Roster[i].LogicPosition;
                if (_awayTeam.Roster.Count > i) transLog.AwayPositions[i] = _awayTeam.Roster[i].LogicPosition;
            }

            MatchLogs.Add(transLog);
            FullMatchLogs.Add(transLog);

            _isTransitionTurn = false;
            _isInboundTurn = false;
            return;
        }

        Vector2 hoopPos = (_currentPossession == TeamSide.Home) ? new Vector2(0.5f, 0.95f) : new Vector2(0.5f, 0.05f);
        float distToHoop = MatchCalculator.CalculateDistance(_ballHolder.LogicPosition, hoopPos);

        TeamTactics attackTactics = MatchDataProxy.Instance.GetTactics(attackTeam.TeamColorId);
        TeamTactics defendTactics = MatchDataProxy.Instance.GetTactics(defendTeam.TeamColorId);

        int action = MatchCalculator.DecideAction(_ballHolder, distToHoop, attackTactics, attackTeam, defendTeam, passInterceptDist, _simTime);

        Debug.Log($"<color=cyan>[턴 진행]</color> 시간:{_simTime:F1} | 볼홀더:{MakeName(_ballHolder.PlayerName)} | 선택행동:{action} (0:슛, 1:패스, 2:드리블)");
        float timeCost = UnityEngine.Random.Range(1f, 5f);
        _simTime -= timeCost;

        if (_simTime <= 0)
        {
            _simTime = 0; // 시간 마이너스 방지

            bool isOT3EndTied = (_simQuarter == 7 && _homeTeam.SimulatedScore == _awayTeam.SimulatedScore);

            if (isOT3EndTied)
            {
                Debug.LogWarning($"[시스템] 연장 3쿼터 무승부 도달! {_ballHolder.PlayerName}의 강제 버저비터 발동!");
                // 강제 버저비터 슛 실행
                DoShoot(_ballHolder, attackTeam, defendTeam, distToHoop, hoopPos, true, attackTactics, defendTactics, true);
            }

            else if (action == 0)
            {
                // 슛을 시도했는데 마침 0초가 됨 -> 버저비터 찬스! (마지막 매개변수 true 전달)
                DoShoot(_ballHolder, attackTeam, defendTeam, distToHoop, hoopPos, true, attackTactics, defendTactics);
            }
        }
        else
        {
            // 시간이 넉넉히 남은 일반적인 상황
            switch (action)
            {
                case 0: DoShoot(_ballHolder, attackTeam, defendTeam, distToHoop, hoopPos, false, attackTactics, defendTactics); break;
                case 1: DoPass(_ballHolder, attackTeam, defendTeam, attackTactics, defendTactics); break;
                case 2: DoDribble(_ballHolder, attackTeam, defendTeam, hoopPos, attackTactics, defendTactics); break;
            }
        }
    }

    private void DoShoot(MatchPlayer shooter, MatchTeam attackTeam, MatchTeam defendTeam, float distToHoop, Vector2 hoopPos, bool isBuzzerBeater, TeamTactics attackTactics, TeamTactics defendTactics, bool forceSuccess = false)
    {
        bool isThree = distToHoop > 0.35f;
        bool isDunk = distToHoop <= 0.05f;

        if (shooter.PassReceivedBuffTick > 0)
        {
            float highlightProb = MatchCalculator.GetSynergyBonus(attackTeam, effectType.HighlightFilm);
            if (highlightProb > 0)
            {
                float hDice = UnityEngine.Random.Range(0f, 100f);
                if (hDice <= highlightProb) // 기획서대로 확률 굴림 (예: 0.1%)
                {
                    isDunk = true;       // 기획서: "덩크 슛 컷 인 이미지 출력" 강제
                    isThree = false;     // 덩크이므로 2점 처리
                    forceSuccess = true; // 기획서: "골 확정"

                    Debug.Log($"<color=#FF00FF>[하이라이트 필름 발동!]</color> 확정 덩크! (주사위: {hDice:F2} <= 확률: {highlightProb:F2}%)");
                }
            }
        }

        int score = isThree ? 3 : 2;

        bool success = forceSuccess || MatchCalculator.CalculateShootSuccess(shooter, distToHoop, attackTeam, defendTeam, attackTactics, defendTactics, blockDist);

        if (isThree) { attackTeam.Try3pt++; if (success) attackTeam.Succ3pt++; }

        // 팀 스탯 기록
        if (isThree) { attackTeam.Try3pt++; if (success) attackTeam.Succ3pt++; }
        else { attackTeam.Try2pt++; if (success) attackTeam.Succ2pt++; }

        // 이벤트 코드 판단
        string eventCode = "";
        if (isBuzzerBeater && success) eventCode = "BuzzerBeater";
        else if (success && isDunk) eventCode = "DunkSucc";
        else if (success && isThree) eventCode = "Shoot3ptSucc";
        else if (success && !isThree) eventCode = "Shoot2ptSucc";
        else if (!success && isThree) eventCode = "Shoot3ptFail";
        else if (!success && !isThree) eventCode = "Shoot2ptFail";

        // Event_Config.csv 테이블에서 가져오기
        var config = _eventConfigReader.DataList.Find(x => x.logEventCode == eventCode);

        // 현재 시간 포맷팅 (MM:SS)
        string timeStr = GetLogTimeStr();

        MatchLogData log = new MatchLogData();
        log.GameTime = Mathf.Max(0, _simTime);
        log.Quarter = _simQuarter;
        log.TeamId = (_currentPossession == TeamSide.Home) ? 0 : 1;
        log.PlayerId = shooter.PlayerId;
        log.PlayerName = MakeName(shooter.PlayerName);
        log.EventType = eventCode;
        log.IsSuccess = success;
        log.ScoreAdded = config != null ? config.scAdd : (success ? score : 0);

        // 슛 결과 텍스트 (시간 + 내용)
        log.BallPos = shooter.LogicPosition;
        // 아군(Home)일 때만 로그 텍스트, 컷인, 사운드를 적용
        if (_currentPossession == TeamSide.Home)
        {
            if (config != null)
            {
                // 텍스트 치환
                string finalText = StringManager.Instance != null ? StringManager.Instance.GetString(config.textTemplate) : config.textTemplate;
                finalText = finalText.Replace("{PlayerName}", log.PlayerName);
                finalText = finalText.Replace("{TeamName}", attackTeam.TeamName);
                string quarterString = _simQuarter > 4
                    ? (StringManager.Instance != null ? StringManager.Instance.GetFormattedString("Str_Match_Overtime", _simQuarter - 4) : $"연장 {_simQuarter - 4}")
                    : _simQuarter.ToString();
                finalText = finalText.Replace("{Quarter}", quarterString);
                log.LogText = $"{timeStr} {finalText}";

                // 사운드 연동
                log.SfxType = string.IsNullOrEmpty(config.soundResourceId) ? "" : config.soundResourceId;

                // 컷인 연동
                log.IsCutIn = !string.IsNullOrEmpty(config.cutInResourceId);
                if (log.IsCutIn)
                {
                    log.CutInType = config.cutInResourceId;
                    if (log.CutInType == "playerCutInResourceId03") log.CutInResourceKey = shooter.CutIn3;
                    else if (log.CutInType == "playerCutInResourceId02") log.CutInResourceKey = shooter.CutIn2;
                    else if (log.CutInType == "playerCutInResourceId01") log.CutInResourceKey = shooter.CutIn1;
                }
                else
                {
                    log.CutInResourceKey = "";
                }
            }
            else
            {
                if (StringManager.Instance != null)
                {
                    log.LogText = success
                        ? StringManager.Instance.GetFormattedString("Str_Match_Fallback_Succ", timeStr, log.PlayerName)
                        : StringManager.Instance.GetFormattedString("Str_Match_Fallback_Fail", timeStr, log.PlayerName);
                }
                else
                {
                    log.LogText = success ? $"{timeStr} {log.PlayerName}이(가) 득점에 성공합니다!" : $"{timeStr} {log.PlayerName}의 슛이 빗나갑니다.";
                }
                log.IsCutIn = false;
            }
        }
        else
        {
            // 적군(Away)일 경우 텍스트, 컷인, 사운드를 모두 비워버림 (기록 안 띄움)
            log.LogText = "";
            log.IsCutIn = false;
            log.CutInType = "";
            log.SfxType = "";
            log.CutInResourceKey = "";
        }

        SavePositionsToLog(log, shooter);
        MatchLogs.Add(log);
        FullMatchLogs.Add(log);

        if (success)
        {
            attackTeam.SimulatedScore += score;
            shooter.Score += score;
            SwitchPossession(false);
            _isTransitionTurn = true;
            _isInboundTurn = true;
            _ballHolder = defendTeam.GetPlayerByPosition(Position.PG) ?? defendTeam.Roster[0];
            Vector2 ourHoop = (_currentPossession == TeamSide.Home) ? new Vector2(0.5f, 0.05f) : new Vector2(0.5f, 0.95f);
            _ballHolder.LogicPosition = ourHoop;

        }
        else
        {
            float rRebBall = MatchDataProxy.Instance.GetBalance("R_Reb_Ball");
            if (rRebBall <= 0f) rRebBall = 0.35f;
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * rRebBall;

            if (hoopPos.y > 0.5f)
            {
                randomOffset.y = -Mathf.Abs(randomOffset.y);
            }
            else
            {
                randomOffset.y = Mathf.Abs(randomOffset.y);
            }

            Vector2 dropPos = new Vector2(
                Mathf.Clamp01(hoopPos.x + randomOffset.x),
                Mathf.Clamp(hoopPos.y + randomOffset.y, 0.05f, 0.95f)
            );

            List<MatchPlayer> allPlayers = new List<MatchPlayer>();
            allPlayers.AddRange(attackTeam.Roster);
            allPlayers.AddRange(defendTeam.Roster);

            TeamTactics homeTactics = MatchDataProxy.Instance.GetTactics(_homeTeam.TeamColorId);
            TeamTactics awayTactics = MatchDataProxy.Instance.GetTactics(_awayTeam.TeamColorId);

            MatchPlayer rebounder = MatchCalculator.CalculateReboundWinner(dropPos, allPlayers, _homeTeam, _awayTeam, homeTactics, awayTactics);
            _ballHolder = rebounder;
            RecordLog("Rebound", rebounder);
            

            // 리바운드 기록
            if (_homeTeam.Roster.Contains(rebounder)) _homeTeam.ReboundCount++;
            else _awayTeam.ReboundCount++;

            if (defendTeam.Roster.Contains(rebounder))
            {
                SwitchPossession(false);
            }
        }
    }

    private void DoPass(MatchPlayer passer, MatchTeam attackTeam, MatchTeam defendTeam, TeamTactics attackTactics, TeamTactics defendTactics)
    {
        MatchPlayer bestReceiver = null;
        float maxPassScore = -999f;

        float penDistHoop = MatchDataProxy.Instance.GetBalance("Pen_Dist_Hoop");
        float wPassBase = MatchDataProxy.Instance.GetBalance("W_Pass_Base");

        // 모든 아군에 대해 계산하여 최적의 패스 대상 판단
        foreach (var mate in attackTeam.Roster)
        {
            if (mate == passer) continue;

            float mateNearestEnemyDist = float.MaxValue;
            foreach (var e in defendTeam.Roster)
            {
                float dist = MatchCalculator.CalculateDistance(mate.LogicPosition, e.LogicPosition);
                if (dist < mateNearestEnemyDist) mateNearestEnemyDist = dist;
            }

            int hasEnemyOnPath = 0;
            float pathEnemySteal = 0f;
            foreach (var e in defendTeam.Roster)
            {
                if (MatchCalculator.DistancePointToLineSegment(e.LogicPosition, passer.LogicPosition, mate.LogicPosition) < passInterceptDist)
                {
                    hasEnemyOnPath = 1;
                    pathEnemySteal = MatchCalculator.GetPlayerStat(e, MatchStatType.Steal, defendTeam);
                    break;
                }
            }

            float currentPassScore = (MatchCalculator.GetPlayerStat(mate, MatchStatType.Pass, attackTeam) * attackTactics.bonusPass * wPassBase)
                                   + (mateNearestEnemyDist * penDistHoop)
                                   - (hasEnemyOnPath * pathEnemySteal * attackTactics.bonusPass);

            if (currentPassScore > maxPassScore)
            {
                maxPassScore = currentPassScore;
                bestReceiver = mate;
            }
        }

        if (bestReceiver == null) return;

        MatchPlayer interceptor = null;
        bool success = MatchCalculator.CalculatePassSuccess(passer, bestReceiver, attackTeam, defendTeam, attackTactics, defendTactics, passInterceptDist, out interceptor);

        // 로그 기록 전 공 소유자 갱신
        if (success)
        {
            _ballHolder = bestReceiver;
            // [시너지용] 패스를 성공적으로 받은 선수에게 3틱짜리 버프 타이머 부여!(패스 받은 직후 4틱 -> 3틱 / 0일때 버프 발동 X)
            bestReceiver.PassReceivedBuffTick = 4;
            RecordLog("PassSucc", passer, bestReceiver);
        }
        else
        {
            _ballHolder = interceptor;
            // 공격팀이 우리 팀(Home)일 때 뺏겼다면 -> PassFail
            if (attackTeam.Side == TeamSide.Home)
            {
                RecordLog("PassFail", passer, interceptor);
            }
            // 공격팀이 적군(Away)일 때 우리가 뺏었다면 -> 스틸 성공
            else
            {
                RecordLog("Steal", interceptor, passer);
            }
            SwitchPossession(false);
        }
    }

    private void DoDribble(MatchPlayer dribbler, MatchTeam attackTeam, MatchTeam defendTeam, Vector2 hoopPos, TeamTactics attackTactics, TeamTactics defendTactics)
    {
        Vector2 dir = (hoopPos - dribbler.LogicPosition).normalized;
        float currentDistToHoop = MatchCalculator.CalculateDistance(dribbler.LogicPosition, hoopPos);

        // 골대를 향해 전진
        float moveDist = Mathf.Min(UnityEngine.Random.Range(0.1f, 0.2f), MAX_MOVE_PER_TICK, currentDistToHoop);
        dribbler.LogicPosition += dir * moveDist;

        // 화면 밖으로 나가지 않도록 보정
        dribbler.LogicPosition = new Vector2(
            Mathf.Clamp01(dribbler.LogicPosition.x),
            Mathf.Clamp(dribbler.LogicPosition.y, 0.05f, 0.95f)
        );
        RecordLog("Dribble", dribbler);
    }

    private void SwitchPossession(bool resetPositions = true)
    {

        MatchTeam currentAttackTeam = (_currentPossession == TeamSide.Home) ? _homeTeam : _awayTeam;
        MatchTeam currentDefendTeam = (_currentPossession == TeamSide.Home) ? _awayTeam : _homeTeam;
        if (resetPositions)
        {
            ResetToDefensePosition(currentAttackTeam);
            ResetToAttackPosition(currentDefendTeam);
        }

        _currentPossession = (_currentPossession == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;
    }

    private void ResetToAttackPosition(MatchTeam team)
    {
        foreach (var player in team.Roster)
        {
            if (player == _ballHolder) continue;
            Vector2 attackPos = GetPreferredPosition(player, true, team.Side);
            player.LogicPosition = attackPos;
        }
    }

    // 파라미터에 target을 추가하여 누구에게 패스하는지 처리할 수 있게 함
    private void RecordLog(string eventCode, MatchPlayer actor = null, MatchPlayer target = null)
    {
        var config = _eventConfigReader.DataList.Find(x => x.logEventCode == eventCode);

        if (config == null || string.IsNullOrEmpty(config.logEventCode))
        {
            Debug.LogWarning($"<color=red>[로그 증발!]</color> Event_Config 테이블에서 '{eventCode}'를 찾을 수 없어서 출력이 누락되었습니다!");
            return;
        }

        // 현재 시간 포맷팅 (MM:SS)
        string timeStr = GetLogTimeStr();

        // 텍스트 치환
        string finalText = StringManager.Instance != null ? StringManager.Instance.GetString(config.textTemplate) : config.textTemplate;
        if (actor != null) finalText = finalText.Replace("{PlayerName}", MakeName(actor.PlayerName));
        if (target != null) finalText = finalText.Replace("{TargetName}", MakeName(target.PlayerName)); // 패스 대상 이름 치환

        // 5쿼터 이상이면 '연장 1', 아니면 원래 숫자 유지
        string quarterString = _simQuarter > 4
            ? (StringManager.Instance != null ? StringManager.Instance.GetFormattedString("Str_Match_Overtime", _simQuarter - 4) : $"연장 {_simQuarter - 4}")
            : _simQuarter.ToString();
        finalText = finalText.Replace("{Quarter}", quarterString);

        MatchLogData log = new MatchLogData();
        log.GameTime = Mathf.Max(0, _simTime);
        log.Quarter = _simQuarter;

        // 최종 텍스트: 시간 + 완성된 문장
        log.LogText = $"{timeStr} {finalText}";
        log.EventType = eventCode;
        log.ScoreAdded = config.scAdd;

        // 사운드 및 컷인 연출 할당
        log.SfxType = string.IsNullOrEmpty(config.soundResourceId) ? "" : config.soundResourceId;
        log.IsCutIn = !string.IsNullOrEmpty(config.cutInResourceId);
        log.CutInType = string.IsNullOrEmpty(config.cutInResourceId) ? "" : config.cutInResourceId;

        if (_ballHolder != null) log.BallPos = _ballHolder.LogicPosition;

        // 행동 주체가 적군(Away) 소속일 경우 모든 연출과 로그 텍스트 삭제
        if (actor != null && _awayTeam.Roster.Contains(actor))
        {
            log.LogText = "";
            log.IsCutIn = false;
            log.CutInType = "";
            log.SfxType = "";
        }
        List<MatchPlayer> stopPlayers = new List<MatchPlayer>();
        if (eventCode == "PassSucc")
        {
            if (actor != null) stopPlayers.Add(actor); // 패스 성공: 패서 1명만 정지
        }
        else if (eventCode == "Steal" || eventCode == "PassFail")
        {
            // 스틸/턴오버: 패스한 사람(actor)과 스틸한 사람(target) 2명 모두 정지
            if (actor != null) stopPlayers.Add(actor);
            if (target != null) stopPlayers.Add(target);
        }
        else
        {
            if (_ballHolder != null) stopPlayers.Add(_ballHolder); // 그 외(슛, 드리블): 볼홀더 정지
        }

        // 선수들을 먼저 이동시킵니다.
        SavePositionsToLog(log, stopPlayers.ToArray());

        // 선수들이 다 이동한 '이후'에 공의 위치를 갱신합니다.
        // 패스 성공 시, 공은 패스받는 target의 '이동이 끝난 새로운 위치'로 갑니다.
        if (eventCode == "PassSucc" && target != null)
        {
            log.BallPos = target.LogicPosition;
        }
        else if (_ballHolder != null)
        {
            log.BallPos = _ballHolder.LogicPosition;
        }
        MatchLogs.Add(log);
        FullMatchLogs.Add(log);
    }

    // 직접 텍스트를 입력하는 버전의 로그 (시간 초과 등)
    private void RecordLog(string text, string type, string sfxType = "")
    {
        string timeStr = GetLogTimeStr();

        MatchLogData log = new MatchLogData();
        log.GameTime = Mathf.Max(0, _simTime);
        log.Quarter = _simQuarter;
        log.LogText = $"{timeStr} {text}"; // 여기도 시간을 맨 앞에 붙임
        log.EventType = type;
        log.SfxType = sfxType;
        if (_ballHolder != null) log.BallPos = _ballHolder.LogicPosition;

        SavePositionsToLog(log, _ballHolder);
        MatchLogs.Add(log);
        FullMatchLogs.Add(log);
    }

    // 10명의 선수를 살짝 이동시키고 좌표를 배열에 담는 함수
    private void SavePositionsToLog(MatchLogData log, params MatchPlayer[] excludedPlayers)
    {
        MoveOffBallPlayers(_homeTeam, excludedPlayers);
        MoveOffBallPlayers(_awayTeam, excludedPlayers);

        for (int i = 0; i < 5; i++)
        {
            if (_homeTeam.Roster.Count > i) log.HomePositions[i] = _homeTeam.Roster[i].LogicPosition;
            if (_awayTeam.Roster.Count > i) log.AwayPositions[i] = _awayTeam.Roster[i].LogicPosition;
        }
    }
    private void ResetToDefensePosition(MatchTeam team)
    {
        foreach (var player in team.Roster)
        {
            Vector2 defensePos = GetPreferredPosition(player, false, team.Side);
            player.LogicPosition = defensePos;
        }
    }
    private void MoveOffBallPlayers(MatchTeam team, params MatchPlayer[] excludedPlayers)
    {
        if (_isTransitionTurn) return;
        bool isAttacking = (team.Side == _currentPossession);

        foreach (var p in team.Roster)
        {
            bool isExcluded = false;
            if (excludedPlayers != null)
            {
                foreach (var ex in excludedPlayers)
                {
                    if (p == ex) { isExcluded = true; break; }
                }
            }
            if (isExcluded) continue;

            Vector2 targetPos = GetPreferredPosition(p, isAttacking, team.Side);

            Vector2 dir = (targetPos - p.LogicPosition);
            float dist = dir.magnitude;

            if (dist > 0.01f)
            {
                float moveDist = Mathf.Min(dist, 0.45f);
                Vector2 move = dir.normalized * moveDist;
                p.LogicPosition += move;
            }

            p.LogicPosition = new Vector2(
                Mathf.Clamp01(p.LogicPosition.x),
                Mathf.Clamp01(p.LogicPosition.y)
            );
        }
    }

    private Vector2 GetPreferredPosition(MatchPlayer player, bool isAttacking, TeamSide side, bool isInitialSetup = false)
    {
        float x = 0.5f;
        float y = 0.5f;
        bool isDataFound = false;

        // 데이터 리더가 있는지 확인
        if (_positionPresetReader != null && _positionPresetReader.DataList != null && _positionPresetReader.DataList.Count > 0)
        {
            // 포지션(TempPositionChange)이 있는지 먼저 검색
            int targetIndex = _positionPresetReader.DataList.FindIndex(data =>
                data.positionType == player.MainPosition &&
                data.changeType == player.TempPositionChange);

            // 빈 데이터인지 체크
            if (targetIndex < 0)
            {
                targetIndex = _positionPresetReader.DataList.FindIndex(data =>
                    data.positionType == player.MainPosition &&
                    data.changeType == changeType.Default);
            }

            // 데이터를 최종적으로 찾았다면 엑셀 범위 내에서 랜덤 지정
            if (targetIndex >= 0)
            {
                var preset = _positionPresetReader.DataList[targetIndex];

                // 좌우 구분이 없는 포지션 (PG, PF, C 등 -> Min과 Min2 값이 동일한 경우)
                if (Mathf.Approximately(preset.offenseXMin, preset.offenseXMin2) && Mathf.Approximately(preset.offenseXMax, preset.offenseXMax2))
                {
                    x = UnityEngine.Random.Range(preset.offenseXMin, preset.offenseXMax);
                }
                // 처음 포지션을 잡을 때 (공수 교대 직후 등)
                else if (isInitialSetup)
                {
                    bool isFirstZone = UnityEngine.Random.value > 0.5f;
                    float minX = isFirstZone ? preset.offenseXMin : preset.offenseXMin2;
                    float maxX = isFirstZone ? preset.offenseXMax : preset.offenseXMax2;
                    x = UnityEngine.Random.Range(minX, maxX);
                }
                // 경기 중 오프볼 이동할 때 -> P_LtoC, P_CtoC 확률 적용 (SG, SF)
                else
                {
                    float centerMin = preset.offenseXMax;
                    float centerMax = preset.offenseXMin2;
                    float currentX = player.LogicPosition.x;

                    // 현재 구역 판별 (0: Left, 1: Center, 2: Right)
                    int currentZone = 1;
                    if (currentX <= centerMin) currentZone = 0;
                    else if (currentX >= centerMax) currentZone = 2;

                    int targetZone = currentZone;

                    float a = preset.P_LtoC;
                    float b = preset.P_CtoC;
                    float rand = UnityEngine.Random.value;

                    if (currentZone == 0) // 현재 왼쪽
                    {
                        if (rand < a) targetZone = 1; // Center로 이동
                    }
                    else if (currentZone == 2) // 현재 오른쪽
                    {
                        if (rand < a) targetZone = 1; // Center로 이동
                    }
                    else // 현재 중앙 (Center)
                    {
                        if (rand > b) // Center에 머물지 않고 이동한다면
                        {
                            // 반반 확률로 왼쪽 또는 오른쪽으로 이동!
                            targetZone = (UnityEngine.Random.value > 0.5f) ? 0 : 2;
                        }
                    }

                    // 결정된 타겟 구역 안에서 랜덤 좌표 픽
                    if (targetZone == 0) x = UnityEngine.Random.Range(preset.offenseXMin, preset.offenseXMax);
                    else if (targetZone == 2) x = UnityEngine.Random.Range(preset.offenseXMin2, preset.offenseXMax2);
                    else x = UnityEngine.Random.Range(centerMin, centerMax);
                }

                y = UnityEngine.Random.Range(preset.offenseYMin, preset.offenseYMax);
                isDataFound = true;
            }
        }

        // 방어 로직
        if (!isDataFound)
        {
            Debug.LogWarning($"[위치 데이터 누락] {player.PlayerName}({player.MainPosition})의 위치 데이터를 엑셀에서 못 찾았습니다! 기본 포메이션으로 임시 배치합니다.");
            switch (player.MainPosition)
            {
                case Position.PG: x = 0.5f; y = 0.65f; break; // 탑
                case Position.SG: x = 0.8f; y = 0.75f; break; // 우측 45도
                case Position.SF: x = 0.2f; y = 0.75f; break; // 좌측 45도
                case Position.PF: x = 0.65f; y = 0.85f; break; // 하이 포스트
                case Position.C: x = 0.5f; y = 0.9f; break;  // 골밑
                default: x = 0.5f; y = 0.5f; break;
            }
            x += UnityEngine.Random.Range(-0.03f, 0.03f);
            y += UnityEngine.Random.Range(-0.03f, 0.03f);
        }

        // 진영(Home/Away) 및 공수(공격/수비) 전환에 따른 Y좌표 대칭 반전
        if (side == TeamSide.Away)
            y = 1.0f - y;

        if (!isAttacking)
            y = 1.0f - y;

        return new Vector2(Mathf.Clamp01(x), Mathf.Clamp01(y));
    }
    private string GetLogTimeStr()
    {
        // 1~4쿼터는 600초(10분), 연장전(5쿼터 이상)은 300초(5분)가 기준
        float maxTime = (_simQuarter > 4) ? 300f : 600f;
        float elapsedTime = maxTime - Mathf.Max(0, _simTime); // 경과 시간 = 총 시간 - 남은 시간

        int minutes = (int)elapsedTime / 60;
        int seconds = (int)elapsedTime % 60;

        return $"{minutes:D2}:{seconds:D2}";
    }

    private string MakeName(string[] nameKey)
    {
        StringManager manager = StringManager.Instance;
        string name = manager.GetString(nameKey[0]) + manager.GetString(nameKey[1]) + manager.GetString(nameKey[2]);
        return name;
    }
    private void MovePlayersForTransitionExcluding(MatchTeam attackTeam, MatchTeam defendTeam, MatchPlayer excluded)
    {
        MoveTeamForTransitionExcluding(attackTeam, true, excluded);
        MoveTeamForTransitionExcluding(defendTeam, false, excluded);
    }

    private void MoveTeamForTransitionExcluding(MatchTeam team, bool isAttacking, MatchPlayer excluded)
    {
        foreach (var p in team.Roster)
        {
            if (p == _ballHolder) continue;

            // 타겟의 Y 방향만 참고하기 위해 호출
            Vector2 targetPos = GetPreferredPosition(p, isAttacking, team.Side);

            // X좌표는 현재 위치(레인)를 그대로 유지하고, Y좌표만 타겟을 향해 최대 0.45 이동
            float dirY = targetPos.y - p.LogicPosition.y;
            float moveY = Mathf.Clamp(dirY, -0.45f, 0.45f);

            p.LogicPosition = new Vector2(
                Mathf.Clamp01(p.LogicPosition.x), // X는 좌우로 안 모이게 현재 위치 고정
                Mathf.Clamp01(p.LogicPosition.y + moveY)
            );
        }
    }
}
