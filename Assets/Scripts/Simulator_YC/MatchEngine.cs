using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchEngine : MonoBehaviour
{
    public List<MatchLogData> MatchLogs = new List<MatchLogData>();

    public Action OnMatchEnded;

    private float _simTime;
    private int _simQuarter;
    private MatchTeam _homeTeam;
    private MatchTeam _awayTeam;
    private TeamSide _currentPossession;
    private MatchPlayer _ballHolder;
    private const float MAX_MOVE_PER_TICK = 1f / 3f; // 기획서 5.3: 틱당 최대 이동거리


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

        // 하프타임 이벤트 패널 대기
        if (uiManager != null)
        {
            uiManager.ShowHalfTimeEvent();
            // 유저가 선택지 버튼을 누를 때까지 무한 대기
            yield return new WaitUntil(() => uiManager.IsEventSelected);

            // 선택한 효과 적용
            state.ApplyHalfTimeEffect(uiManager.SelectedEventIndex);
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
    }

    public void CalculateUntilQuarter(int targetQuarter)
    {
        MatchLogs.Clear();

        Debug.Log($">>> [MatchEngine] Simulation Phase: {_simQuarter}Q ~ {targetQuarter}Q");

        // 일반 쿼터 처리 (1~4쿼터)
        while (_simQuarter <= targetQuarter)
        {
            RecordLog($"=== Quarter {_simQuarter} Start ===", "QUARTER_START");

            while (_simTime > 0)
            {
                ProcessTurn();
            }

            RecordLog($"--- Quarter {_simQuarter} Ended ---", "QUARTER_END");

            _simQuarter++;
            _simTime = 600f;
            _currentPossession = (_currentPossession == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;
        }

        // 연장전 처리 (4쿼터가 끝났는데 동점일 때만)
        if (targetQuarter >= 4)
        {
            while (_homeTeam.Score == _awayTeam.Score)
            {
                RecordLog($"=== Overtime {_simQuarter - 4} Start ===", "QUARTER_START");

                _simTime = 300f; // ← 이 값은 기획팀 확인 필요! (연장전 쿼터당 몇 분인지 미명시)

                while (_simTime > 0)
                {
                    ProcessTurn();
                }

                RecordLog($"--- Overtime {_simQuarter - 4} Ended ---", "QUARTER_END");

                _simQuarter++;
                _currentPossession = (_currentPossession == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;
            }

            RecordLog("=== Match Ended ===", "MATCH_END");
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

        TeamTactics tactics = MatchDataProxy.Instance.GetTactics(attackTeam.TeamColorId);

        int action = MatchCalculator.DecideAction(_ballHolder, distToHoop, tactics, attackTeam.Roster, defendTeam.Roster);

        float timeCost = UnityEngine.Random.Range(5f, 10f);
        _simTime -= timeCost;

        if (_simTime <= 0)
        {
            _simTime = 0; // 시간 마이너스 방지

            if (action == 0)
            {
                // 슛을 시도했는데 마침 0초가 됨 -> 버저비터 찬스! (마지막 매개변수 true 전달)
                DoShoot(_ballHolder, attackTeam, defendTeam, distToHoop, hoopPos, true);
            }
            else
            {
                // 패스나 드리블 중에 시간이 끝남 -> 공격 무산 및 쿼터 종료
                RecordLog("시간 초과! 공격이 무산되며 쿼터가 종료됩니다.", "TIME_OVER");
            }
        }
        else
        {
            // 시간이 넉넉히 남은 일반적인 상황
            switch (action)
            {
                case 0: DoShoot(_ballHolder, attackTeam, defendTeam, distToHoop, hoopPos, false); break;
                case 1: DoPass(_ballHolder, attackTeam, defendTeam); break;
                case 2: DoDribble(_ballHolder, defendTeam.Roster, hoopPos); break;
            }
        }
    }

    private void DoShoot(MatchPlayer shooter, MatchTeam attackTeam, MatchTeam defendTeam, float distance, Vector2 hoopPos, bool isBuzzerBeater)
    {
        bool isThree = distance > 0.35f;
        bool isDunk = distance <= 0.05f;

        int score = isThree ? 3 : 2;

        bool success = MatchCalculator.CalculateShootSuccess(shooter, distance, attackTeam, defendTeam);

        MatchLogData log = new MatchLogData();
        log.GameTime = Mathf.Max(0, _simTime);
        log.Quarter = _simQuarter;
        log.TeamId = (_currentPossession == TeamSide.Home) ? 0 : 1;
        log.PlayerId = shooter.PlayerId;
        log.PlayerName = shooter.PlayerName;
        log.EventType = success ? "GOAL" : "MISS";
        log.IsSuccess = success;
        log.ScoreAdded = success ? score : 0;
        log.LogText = success ? $"{shooter.PlayerName} scored!" : $"{shooter.PlayerName} missed.";
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

        MatchLogs.Add(log);

        if (success)
        {
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
            RecordLog($"Rebound grabbed by {rebounder.PlayerName}!", "REBOUND");
            _ballHolder = rebounder;

            if (defendTeam.Roster.Contains(rebounder)) SwitchPossession(false); // 리바운드 후에도 자리 리셋 없이 진행하려면 false
        }
    }

    private void DoPass(MatchPlayer passer, MatchTeam attackTeam, MatchTeam defendTeam)
    {
        MatchPlayer receiver = attackTeam.Roster.Find(p => p != passer);
        if (receiver == null) return;

        MatchPlayer interceptor;
        bool success = MatchCalculator.CalculatePassSuccess(passer, receiver, defendTeam.Roster, out interceptor);

        if (success) { RecordLog($"Pass to {receiver.PlayerName}", "PASS"); _ballHolder = receiver; }
        else
        {
            // 스틸(패스 차단) 성공 시 박수 사운드 예약
            RecordLog($"Pass Intercepted by {interceptor.PlayerName}!", "STEAL", "CLAP");
            SwitchPossession();
            _ballHolder = interceptor;
        }
    }

    private void DoDribble(MatchPlayer dribbler, List<MatchPlayer> enemies, Vector2 hoopPos)
    {
        bool success = MatchCalculator.CalculateDribbleSuccess(dribbler, enemies);
        if (success)
        {
            Vector2 dir = (hoopPos - dribbler.LogicPosition).normalized;
            float moveDist = Mathf.Min(UnityEngine.Random.Range(0.1f, 0.2f), MAX_MOVE_PER_TICK);
            dribbler.LogicPosition += dir * moveDist;
            RecordLog($"{dribbler.PlayerName} drives successfully!", "DRIBBLE");
        }
        else
        {
            Vector2 sideDir = new Vector2(UnityEngine.Random.value > 0.5f ? 1 : -1, 0);
            float moveDist = Mathf.Min(0.1f, MAX_MOVE_PER_TICK);
            dribbler.LogicPosition += sideDir * moveDist;
            RecordLog($"{dribbler.PlayerName} blocked, moves to side.", "BLOCK");
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



    private void RecordLog(string text, string type, string sfxType = "")
    {
        MatchLogData log = new MatchLogData();
        log.GameTime = Mathf.Max(0, _simTime);
        log.Quarter = _simQuarter;
        log.LogText = text;
        log.EventType = type;
        log.SfxType = sfxType;
        if (_ballHolder != null) log.BallPos = _ballHolder.LogicPosition;

        SavePositionsToLog(log);
        MatchLogs.Add(log);
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
        float x, y;

        switch (player.MainPosition)
        {
            case Position.PG:
                x = UnityEngine.Random.Range(0.3f, 0.7f);
                y = UnityEngine.Random.Range(0.60f, 0.75f);
                break;
            case Position.SG:
                x = (player.LogicPosition.x < 0.5f)
                    ? UnityEngine.Random.Range(0.1f, 0.3f)
                    : UnityEngine.Random.Range(0.7f, 0.9f);
                y = UnityEngine.Random.Range(0.70f, 0.85f);
                break;
            case Position.SF:
                x = (player.LogicPosition.x < 0.5f)
                    ? UnityEngine.Random.Range(0.05f, 0.2f)
                    : UnityEngine.Random.Range(0.8f, 0.95f);
                y = UnityEngine.Random.Range(0.75f, 0.90f);
                break;
            case Position.PF:
                x = UnityEngine.Random.Range(0.35f, 0.65f);
                y = UnityEngine.Random.Range(0.80f, 0.90f);
                break;
            case Position.C:
                x = UnityEngine.Random.Range(0.40f, 0.60f);
                y = UnityEngine.Random.Range(0.85f, 0.95f);
                break;
            default:
                x = 0.5f;
                y = 0.5f;
                break;
        }

        // 어웨이팀은 공격 방향이 반대(아래)이므로 y 반전
        if (side == TeamSide.Away)
            y = 1.0f - y;

        // 수비 시 추가로 y 반전
        if (!isAttacking)
            y = 1.0f - y;

        return new Vector2(x, y);
    }


}
