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

    [Header("Data Readers")]
    [SerializeField] private Event_ConfigDataReader _eventConfigReader;
    [SerializeField] private Position_PresetDataReader _positionPresetReader;
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
        FullMatchLogs.Clear(); // 새 경기 시작 시 전체 로그 초기화

        _homeTeam.SimulatedScore = 0;
        _awayTeam.SimulatedScore = 0;
    }

    public void CalculateUntilQuarter(int targetQuarter)
    {
        MatchLogs.Clear();

        Debug.Log($">>> [MatchEngine] Simulation Phase: {_simQuarter}Q ~ {targetQuarter}Q");

        // 일반 쿼터 처리 (1~4쿼터)
        while (_simQuarter <= targetQuarter)
        {
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

        // 연장전 처리 (4쿼터가 끝났는데 동점일 때만)
        if (targetQuarter >= 4)
        {
            while (_homeTeam.SimulatedScore == _awayTeam.SimulatedScore)
            {
                RecordLog("GameStart");

                _simTime = 300f;

                while (_simTime > 0)
                {
                    ProcessTurn();
                }

                RecordLog("QuarterEnd");

                _simQuarter++;
                _currentPossession = (_currentPossession == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;
            }

            RecordLog("GameEnd");
        }
    }

    private void ProcessTurn()
    {

        MatchTeam attackTeam = (_currentPossession == TeamSide.Home) ? _homeTeam : _awayTeam;
        MatchTeam defendTeam = (_currentPossession == TeamSide.Home) ? _awayTeam : _homeTeam;

        if (_ballHolder == null || !attackTeam.Roster.Contains(_ballHolder))
            _ballHolder = attackTeam.GetPlayerByPosition(Position.PG) ?? attackTeam.Roster[0];

        Vector2 hoopPos = (_currentPossession == TeamSide.Home) ? new Vector2(0.5f, 0.95f) : new Vector2(0.5f, 0.05f);
        float distToHoop = MatchCalculator.CalculateDistance(_ballHolder.LogicPosition, hoopPos);

        TeamTactics attackTactics = MatchDataProxy.Instance.GetTactics(attackTeam.TeamColorId);
        TeamTactics defendTactics = MatchDataProxy.Instance.GetTactics(defendTeam.TeamColorId);

        int action = MatchCalculator.DecideAction(_ballHolder, distToHoop, attackTactics, attackTeam.Roster, defendTeam.Roster);

        float timeCost = UnityEngine.Random.Range(5f, 10f);
        _simTime -= timeCost;

        if (_simTime <= 0)
        {
            _simTime = 0; // 시간 마이너스 방지

            if (action == 0)
            {
                // 슛을 시도했는데 마침 0초가 됨 -> 버저비터 찬스! (마지막 매개변수 true 전달)
                DoShoot(_ballHolder, attackTeam, defendTeam, distToHoop, hoopPos, true, attackTactics, defendTactics);
            }
            else
            {
                // 패스나 드리블 중에 시간이 끝남 -> 공격 무산 및 쿼터 종료
                RecordLog("공격이 무산되며 쿼터가 종료됩니다.", "TIME_OVER");
            }
        }
        else
        {
            // 시간이 넉넉히 남은 일반적인 상황
            switch (action)
            {
                case 0: DoShoot(_ballHolder, attackTeam, defendTeam, distToHoop, hoopPos, false, attackTactics, defendTactics); break;
                case 1: DoPass(_ballHolder, attackTeam, defendTeam, attackTactics, defendTactics); break;
                case 2: DoDribble(_ballHolder, defendTeam.Roster, hoopPos, attackTactics, defendTactics); break;
            }
        }
    }

    private void DoShoot(MatchPlayer shooter, MatchTeam attackTeam, MatchTeam defendTeam, float distance, Vector2 hoopPos, bool isBuzzerBeater, TeamTactics attackTactics, TeamTactics defendTactics)
    {
        bool isThree = distance > 0.35f;
        bool isDunk = distance <= 0.05f;

        int score = isThree ? 3 : 2;

        bool success = MatchCalculator.CalculateShootSuccess(shooter, distance, attackTeam, defendTeam, attackTactics, defendTactics);

        // 팀 스탯 기록
        if (isThree) { attackTeam.Try3pt++; if (success) attackTeam.Succ3pt++; }
        else { attackTeam.Try2pt++; if (success) attackTeam.Succ2pt++; }

        // 현재 시간 포맷팅 (MM:SS)
        string timeStr = GetLogTimeStr();

        MatchLogData log = new MatchLogData();
        log.GameTime = Mathf.Max(0, _simTime);
        log.Quarter = _simQuarter;
        log.TeamId = (_currentPossession == TeamSide.Home) ? 0 : 1;
        log.PlayerId = shooter.PlayerId;
        log.PlayerName = shooter.PlayerName;
        log.EventType = success ? "GOAL" : "MISS";
        log.IsSuccess = success;
        log.ScoreAdded = success ? score : 0;

        // 슛 결과 텍스트 (시간 + 내용)
        log.LogText = success ? $"{timeStr} {shooter.PlayerName}이(가) 득점에 성공합니다!" : $"{timeStr} {shooter.PlayerName}의 슛이 빗나갑니다.";
        log.BallPos = shooter.LogicPosition;

        // 버저비터를 먼저 체크하고, 아닐 때만 덩크/3점 체크
        if (isBuzzerBeater && success)
        {
            log.IsCutIn = true;
            log.CutInType = "BUZZER";
        }
        else if (success && isDunk)
        {
            log.IsCutIn = true;
            log.CutInType = "DUNK";
        }
        else if (success && isThree)
        {
            log.IsCutIn = true;
            log.CutInType = "3PT";
        }
        else
        {
            log.IsCutIn = false;
            log.CutInType = "";
        }

        if (success && !isThree && !isDunk && _simTime > 0)
        {
            log.SfxType = "CHEER";
        }
        SavePositionsToLog(log);
        MatchLogs.Add(log);
        FullMatchLogs.Add(log);

        if (success)
        {
            attackTeam.SimulatedScore += score;
            SwitchPossession(false);
            _ballHolder = defendTeam.GetPlayerByPosition(Position.PG) ?? defendTeam.Roster[0];
            Vector2 ourHoop = (_currentPossession == TeamSide.Home) ? new Vector2(0.5f, 0.05f) : new Vector2(0.5f, 0.95f);
            _ballHolder.LogicPosition = ourHoop;
        }
        else
        {
            float yMin = (_currentPossession == TeamSide.Home) ? hoopPos.y - 0.2f : hoopPos.y;
            float yMax = (_currentPossession == TeamSide.Home) ? hoopPos.y : hoopPos.y + 0.2f;
            float yDropOffset = (_currentPossession == TeamSide.Home) ?
                    UnityEngine.Random.Range(-0.2f, 0.0f) :  // 홈팀 공격 시: 골대(0.95)보다 아래로 떨어짐
                    UnityEngine.Random.Range(0.0f, 0.2f);    // 어웨이 공격 시: 골대(0.05)보다 위로 떨어짐

            Vector2 dropPos = hoopPos + new Vector2(UnityEngine.Random.Range(-0.2f, 0.2f), yDropOffset);
            List<MatchPlayer> allPlayers = new List<MatchPlayer>();
            allPlayers.AddRange(attackTeam.Roster);
            allPlayers.AddRange(defendTeam.Roster);

            MatchPlayer rebounder = MatchCalculator.CalculateReboundWinner(dropPos, allPlayers);
            RecordLog("Rebound", rebounder);
            _ballHolder = rebounder;

            // 리바운드 기록
            if (_homeTeam.Roster.Contains(rebounder)) _homeTeam.ReboundCount++;
            else _awayTeam.ReboundCount++;

            if (defendTeam.Roster.Contains(rebounder)) SwitchPossession(false);
        }
    }

    private void DoPass(MatchPlayer passer, MatchTeam attackTeam, MatchTeam defendTeam, TeamTactics attackTactics, TeamTactics defendTactics)
    {
        MatchPlayer bestReceiver = null;
        float maxPassScore = -999f;

        float interceptDist = MatchDataProxy.Instance.GetBalance("Pen_Intercept_Dist");
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
                if (MatchCalculator.DistancePointToLineSegment(e.LogicPosition, passer.LogicPosition, mate.LogicPosition) < interceptDist)
                {
                    hasEnemyOnPath = 1;
                    pathEnemySteal = e.GetStat(MatchStatType.Steal);
                    break;
                }
            }

            float currentPassScore = (mate.GetStat(MatchStatType.Pass) * attackTactics.bonusPass * wPassBase)
                                   + (mateNearestEnemyDist * penDistHoop)
                                   - (hasEnemyOnPath * pathEnemySteal * attackTactics.bonusPass);

            if (currentPassScore > maxPassScore)
            {
                maxPassScore = currentPassScore;
                bestReceiver = mate;
            }
        }

        if (bestReceiver == null) return;

        MatchPlayer interceptor;
        bool success = MatchCalculator.CalculatePassSuccess(passer, bestReceiver, attackTeam, defendTeam, attackTactics, defendTactics, out interceptor);

        // 로그 기록 전 공 소유자 갱신
        if (success)
        {
            _ballHolder = bestReceiver;
            RecordLog("PassSucc", passer, bestReceiver);
        }
        else
        {
            _ballHolder = interceptor;
            RecordLog("Steal", interceptor);
            SwitchPossession();
        }
    }

    private void DoDribble(MatchPlayer dribbler, List<MatchPlayer> enemies, Vector2 hoopPos, TeamTactics attackTactics, TeamTactics defendTactics)
    {
        bool success = MatchCalculator.CalculateDribbleSuccess(dribbler, enemies, attackTactics, defendTactics);
        if (success)
        {
            Vector2 dir = (hoopPos - dribbler.LogicPosition).normalized;
            float moveDist = Mathf.Min(UnityEngine.Random.Range(0.1f, 0.2f), MAX_MOVE_PER_TICK);
            dribbler.LogicPosition += dir * moveDist;
            RecordLog("Dribble", dribbler);
        }
        else
        {
            Vector2 sideDir = new Vector2(UnityEngine.Random.value > 0.5f ? 1 : -1, 0);
            float moveDist = Mathf.Min(0.1f, MAX_MOVE_PER_TICK);
            dribbler.LogicPosition += sideDir * moveDist;
            RecordLog("Block", dribbler);
        }
        dribbler.LogicPosition = new Vector2(Mathf.Clamp01(dribbler.LogicPosition.x), Mathf.Clamp01(dribbler.LogicPosition.y));
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
            Debug.LogWarning($"[MatchEngine] Event_Config 테이블에서 '{eventCode}'를 찾을 수 없습니다.");
            return;
        }

        // 현재 시간 포맷팅 (MM:SS)
        string timeStr = GetLogTimeStr();

        // 텍스트 치환
        string finalText = StringManager.Instance != null ? StringManager.Instance.GetString(config.textTemplate) : config.textTemplate;
        if (actor != null) finalText = finalText.Replace("{PlayerName}", actor.PlayerName);
        if (target != null) finalText = finalText.Replace("{TargetName}", target.PlayerName); // 패스 대상 이름 치환
        // 5쿼터 이상이면 '연장 1', 아니면 원래 숫자 유지
        string quarterString = _simQuarter > 4 ? $"연장 {_simQuarter - 4}" : _simQuarter.ToString();
        finalText = finalText.Replace("{Quarter}", quarterString);

        MatchLogData log = new MatchLogData();
        log.GameTime = Mathf.Max(0, _simTime);
        log.Quarter = _simQuarter;

        // 최종 텍스트: 시간 + 완성된 문장
        log.LogText = $"{timeStr} {finalText}";
        log.EventType = eventCode;
        log.ScoreAdded = config.scAdd;

        // 사운드 및 컷인 연출 할당
        log.SfxType = config.soundResourceId == "-" ? "" : config.soundResourceId;
        log.IsCutIn = config.cutInResourceId != "-";
        log.CutInType = config.cutInResourceId == "-" ? "" : config.cutInResourceId;

        if (_ballHolder != null) log.BallPos = _ballHolder.LogicPosition;

        SavePositionsToLog(log);
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

        SavePositionsToLog(log);
        MatchLogs.Add(log);
        FullMatchLogs.Add(log);
    }

    // 10명의 선수를 살짝 이동시키고 좌표를 배열에 담는 함수
    private void SavePositionsToLog(MatchLogData log)
    {
        MoveOffBallPlayers(_homeTeam);
        MoveOffBallPlayers(_awayTeam);

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
    private void MoveOffBallPlayers(MatchTeam team)
    {
        bool isAttacking = (team.Side == _currentPossession);

        foreach (var p in team.Roster)
        {
            if (p == _ballHolder) continue;

            Vector2 targetPos = GetPreferredPosition(p, isAttacking, team.Side);

            Vector2 dir = (targetPos - p.LogicPosition);
            float dist = dir.magnitude;

            if (dist > 0.01f)
            {
                Vector2 move = dir.normalized * Mathf.Min(dist, MAX_MOVE_PER_TICK);
                p.LogicPosition += move;
            }

            p.LogicPosition = new Vector2(
                Mathf.Clamp01(p.LogicPosition.x),
                Mathf.Clamp01(p.LogicPosition.y)
            );
        }
    }

    private Vector2 GetPreferredPosition(MatchPlayer player, bool isAttacking, TeamSide side)
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
                x = UnityEngine.Random.Range(preset.offenseXMin, preset.offenseXMax);
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
        }

        // 진영(Home/Away) 및 공수(공격/수비) 전환에 따른 Y좌표 대칭 반전
        if (side == TeamSide.Away)
            y = 1.0f - y;

        if (!isAttacking)
            y = 1.0f - y;

        x += UnityEngine.Random.Range(-0.03f, 0.03f);
        y += UnityEngine.Random.Range(-0.03f, 0.03f);

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
}
