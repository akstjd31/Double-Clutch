using System.Collections.Generic;
using UnityEngine;

public class HeadlessMatchSimulator : MonoBehaviour
{
    private float _simTime;
    private int _simQuarter;
    private MatchTeam _homeTeam;
    private MatchTeam _awayTeam;
    private TeamSide _currentPossession;
    private MatchPlayer _ballHolder;

    private const float MAX_MOVE_PER_TICK = 1f / 3f; // 기획서 5.3: 틱당 최대 이동거리

    [Header("Data Readers")]
    [SerializeField] private Position_PresetDataReader _positionPresetReader;
    [SerializeField] private Player_SynergyDataReader _synergyReader;

    [Header("Balance Settings")]
    [SerializeField] private float dribbleBlockDist = 0.1f;
    [SerializeField] private float passInterceptDist = 0.03f;
    [SerializeField] private float blockDist = 0.25f;

    // 매 턴 FindIndex로 리스트를 뒤지지 않게 포지션별 Default 프리셋만 저장해두는 캐시
    private Dictionary<Position, Position_PresetData> _presetCache = new Dictionary<Position, Position_PresetData>();

    // 외부(리그 매니저)에서 NPC 매치를 돌릴 때 호출할 함수
    public (int homeScore, int awayScore) SimulateNPCMatch(MatchTeam home, MatchTeam away)
    {
        _homeTeam = home;
        _awayTeam = away;
        _simQuarter = 1;
        _currentPossession = TeamSide.Home;
        _homeTeam.SimulatedScore = 0;
        _awayTeam.SimulatedScore = 0;

        // 시너지 평가 (원본과 동일하게 시작 전 모든 시너지 활성화)
        if (_synergyReader != null)
        {
            _homeTeam.EvaluateSynergies(_synergyReader.DataList);
            _awayTeam.EvaluateSynergies(_synergyReader.DataList);
        }

        // 렉 방지용 프리셋 캐싱
        BuildPresetCache();

        // 어웨이 팀 좌표 반전
        foreach (var player in _awayTeam.Roster)
        {
            if (player.LogicPosition.y >= 0.5f)
                player.LogicPosition = new Vector2(player.LogicPosition.x, 1.0f - player.LogicPosition.y);
        }

        // 전후반전(1~4쿼터) 연산
        while (_simQuarter <= 4)
        {
            RunQuarter(600f);
            _simQuarter++;
            _currentPossession = (_currentPossession == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;
        }

        // 연장전 연산
        while (_homeTeam.SimulatedScore == _awayTeam.SimulatedScore && _simQuarter <= 7)
        {
            RunQuarter(300f); // 연장전은 5분(300초)
            _simQuarter++;
            _currentPossession = (_currentPossession == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;
        }

        return (_homeTeam.SimulatedScore, _awayTeam.SimulatedScore);
    }

    private void BuildPresetCache()
    {
        if (_positionPresetReader == null || _positionPresetReader.DataList == null) return;
        _presetCache.Clear();

        // NPC는 하프타임 변경이 없으므로 오직 Default 상태의 프리셋만 1번 찾아둠
        foreach (Position pos in System.Enum.GetValues(typeof(Position)))
        {
            if (pos == Position.None) continue;
            int targetIndex = _positionPresetReader.DataList.FindIndex(data =>
                data.positionType == pos && data.changeType == changeType.Default);

            if (targetIndex >= 0)
            {
                _presetCache[pos] = _positionPresetReader.DataList[targetIndex];
            }
        }
    }

    private void RunQuarter(float quarterTime)
    {
        _simTime = quarterTime;

        while (_simTime > 0)
        {
            // [시너지용] 매 턴(틱) 버프 감소
            foreach (var p in _homeTeam.Roster) { if (p.PassReceivedBuffTick > 0) p.PassReceivedBuffTick--; }
            foreach (var p in _awayTeam.Roster) { if (p.PassReceivedBuffTick > 0) p.PassReceivedBuffTick--; }

            MatchTeam attackTeam = (_currentPossession == TeamSide.Home) ? _homeTeam : _awayTeam;
            MatchTeam defendTeam = (_currentPossession == TeamSide.Home) ? _awayTeam : _homeTeam;

            if (_ballHolder == null || !attackTeam.Roster.Contains(_ballHolder))
                _ballHolder = attackTeam.GetPlayerByPosition(Position.PG) ?? attackTeam.Roster[0];

            // 오프볼 플레이어들 위치 이동 연산 (로그가 빠졌으므로 여기서 직접 갱신)
            UpdateOffBallPlayers(attackTeam, defendTeam);

            Vector2 hoopPos = (_currentPossession == TeamSide.Home) ? new Vector2(0.5f, 0.95f) : new Vector2(0.5f, 0.05f);
            float distToHoop = MatchCalculator.CalculateDistance(_ballHolder.LogicPosition, hoopPos);

            TeamTactics attackTactics = MatchDataProxy.Instance.GetTactics(attackTeam.TeamColorId);
            TeamTactics defendTactics = MatchDataProxy.Instance.GetTactics(defendTeam.TeamColorId);

            int action = MatchCalculator.DecideAction(_ballHolder, distToHoop, attackTactics, attackTeam, defendTeam, passInterceptDist, _simTime);

            _simTime -= UnityEngine.Random.Range(1f, 3f);

            // 시간 종료 시 연장 3쿼터 무승부 강제 버저비터 로직
            if (_simTime <= 0)
            {
                _simTime = 0;
                bool isOT3EndTied = (_simQuarter == 7 && _homeTeam.SimulatedScore == _awayTeam.SimulatedScore);

                if (isOT3EndTied)
                {
                    DoShoot(_ballHolder, attackTeam, defendTeam, distToHoop, hoopPos, true, attackTactics, defendTactics, true);
                }
                else if (action == 0)
                {
                    DoShoot(_ballHolder, attackTeam, defendTeam, distToHoop, hoopPos, true, attackTactics, defendTactics, false);
                }
                break;
            }

            if (action == 0) DoShoot(_ballHolder, attackTeam, defendTeam, distToHoop, hoopPos, false, attackTactics, defendTactics, false);
            else if (action == 1) DoPass(_ballHolder, attackTeam, defendTeam, attackTactics, defendTactics);
            else if (action == 2) DoDribble(_ballHolder, attackTeam, defendTeam, hoopPos, attackTactics, defendTactics);
        }
    }

    private void DoShoot(MatchPlayer shooter, MatchTeam attackTeam, MatchTeam defendTeam, float distance, Vector2 hoopPos, bool isBuzzerBeater, TeamTactics atkTac, TeamTactics defTac, bool forceSuccess)
    {
        bool isThree = distance > 0.35f;
        int score = isThree ? 3 : 2;

        // 하이라이트 필름 시너지 연산
        if (shooter.PassReceivedBuffTick > 0)
        {
            float highlightProb = MatchCalculator.GetSynergyBonus(attackTeam, effectType.HighlightFilm);
            if (highlightProb > 0 && UnityEngine.Random.Range(0f, 100f) <= highlightProb) forceSuccess = true;
        }

        // MatchCalculator의 슛 성공률 
        bool success = forceSuccess || MatchCalculator.CalculateShootSuccess(shooter, distance, attackTeam, defendTeam, atkTac, defTac, blockDist);

        if (isThree) { attackTeam.Try3pt++; if (success) attackTeam.Succ3pt++; }
        else { attackTeam.Try2pt++; if (success) attackTeam.Succ2pt++; }

        if (success)
        {
            attackTeam.SimulatedScore += score;
            shooter.Score += score;
            SwitchPossession(false);

            _ballHolder = defendTeam.GetPlayerByPosition(Position.PG) ?? defendTeam.Roster[0];
            Vector2 ourHoop = (_currentPossession == TeamSide.Home) ? new Vector2(0.5f, 0.05f) : new Vector2(0.5f, 0.95f);
            _ballHolder.LogicPosition = ourHoop;
        }
        else
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 0.35f;
            randomOffset.y /= 1.87f;
            if (hoopPos.y > 0.5f) randomOffset.y = -Mathf.Abs(randomOffset.y);
            else randomOffset.y = Mathf.Abs(randomOffset.y);

            Vector2 dropPos = new Vector2(Mathf.Clamp01(hoopPos.x + randomOffset.x), Mathf.Clamp01(hoopPos.y + randomOffset.y));

            List<MatchPlayer> allPlayers = new List<MatchPlayer>();
            allPlayers.AddRange(attackTeam.Roster);
            allPlayers.AddRange(defendTeam.Roster);

            MatchPlayer rebounder = MatchCalculator.CalculateReboundWinner(dropPos, allPlayers, _homeTeam, _awayTeam, atkTac, defTac);
            _ballHolder = rebounder;

            if (_homeTeam.Roster.Contains(rebounder)) _homeTeam.ReboundCount++;
            else _awayTeam.ReboundCount++;

            if (defendTeam.Roster.Contains(rebounder)) SwitchPossession(false);
        }
    }

    private void DoPass(MatchPlayer passer, MatchTeam attackTeam, MatchTeam defendTeam, TeamTactics atkTac, TeamTactics defTac)
    {
        // 패스 능력치
        MatchPlayer bestReceiver = null;
        float maxPassScore = -999f;
        float penDistHoop = MatchDataProxy.Instance.GetBalance("Pen_Dist_Hoop");
        float wPassBase = MatchDataProxy.Instance.GetBalance("W_Pass_Base");

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

            float currentPassScore = (MatchCalculator.GetPlayerStat(mate, MatchStatType.Pass, attackTeam) * atkTac.bonusPass * wPassBase)
                                   + (mateNearestEnemyDist * penDistHoop)
                                   - (hasEnemyOnPath * pathEnemySteal * atkTac.bonusPass);

            if (currentPassScore > maxPassScore)
            {
                maxPassScore = currentPassScore;
                bestReceiver = mate;
            }
        }

        if (bestReceiver == null) return;

        MatchPlayer interceptor;
        bool success = MatchCalculator.CalculatePassSuccess(passer, bestReceiver, attackTeam, defendTeam, atkTac, defTac, passInterceptDist, out interceptor);

        if (success)
        {
            _ballHolder = bestReceiver;
            bestReceiver.PassReceivedBuffTick = 4;
        }
        else
        {
            _ballHolder = interceptor;
            SwitchPossession(true);
        }
    }

    private void DoDribble(MatchPlayer dribbler, MatchTeam attackTeam, MatchTeam defendTeam, Vector2 hoopPos, TeamTactics atkTac, TeamTactics defTac)
    {
        bool success = MatchCalculator.CalculateDribbleSuccess(dribbler, attackTeam, defendTeam, atkTac, defTac, dribbleBlockDist);
        Vector2 dir = (hoopPos - dribbler.LogicPosition).normalized;

        if (success) dribbler.LogicPosition += dir * Mathf.Min(UnityEngine.Random.Range(0.1f, 0.2f), MAX_MOVE_PER_TICK);
        else dribbler.LogicPosition += new Vector2(UnityEngine.Random.value > 0.5f ? 1 : -1, 0) * Mathf.Min(0.1f, MAX_MOVE_PER_TICK);

        dribbler.LogicPosition = new Vector2(Mathf.Clamp01(dribbler.LogicPosition.x), Mathf.Clamp01(dribbler.LogicPosition.y));
    }

    private void SwitchPossession(bool resetPositions = true)
    {
        MatchTeam currentAttackTeam = (_currentPossession == TeamSide.Home) ? _homeTeam : _awayTeam;
        MatchTeam currentDefendTeam = (_currentPossession == TeamSide.Home) ? _awayTeam : _homeTeam;

        if (resetPositions)
        {
            foreach (var player in currentAttackTeam.Roster) player.LogicPosition = GetPreferredPosition(player, false, currentAttackTeam.Side);
            foreach (var player in currentDefendTeam.Roster) player.LogicPosition = GetPreferredPosition(player, true, currentDefendTeam.Side);
        }

        _currentPossession = (_currentPossession == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;
    }

    private void UpdateOffBallPlayers(MatchTeam attackTeam, MatchTeam defendTeam)
    {
        MoveOffBallTeam(attackTeam, true);
        MoveOffBallTeam(defendTeam, false);
    }

    private void MoveOffBallTeam(MatchTeam team, bool isAttacking)
    {
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

            p.LogicPosition = new Vector2(Mathf.Clamp01(p.LogicPosition.x), Mathf.Clamp01(p.LogicPosition.y));
        }
    }
    private Vector2 GetPreferredPosition(MatchPlayer player, bool isAttacking, TeamSide side)
    {
        float x = 0.5f, y = 0.5f;

        if (_presetCache.TryGetValue(player.MainPosition, out Position_PresetData preset))
        {
            x = UnityEngine.Random.Range(preset.offenseXMin, preset.offenseXMax);
            y = UnityEngine.Random.Range(preset.offenseYMin, preset.offenseYMax);
        }
        else
        {
            // 예비 좌표 (만약 엑셀 데이터가 누락되었을 때 대비)
            switch (player.MainPosition)
            {
                case Position.PG: x = 0.5f; y = 0.65f; break;
                case Position.SG: x = 0.8f; y = 0.75f; break;
                case Position.SF: x = 0.2f; y = 0.75f; break;
                case Position.PF: x = 0.65f; y = 0.85f; break;
                case Position.C: x = 0.5f; y = 0.9f; break;
                default: x = 0.5f; y = 0.5f; break;
            }
        }

        if (side == TeamSide.Away) y = 1.0f - y;
        if (!isAttacking) y = 1.0f - y;

        x += UnityEngine.Random.Range(-0.03f, 0.03f);
        y += UnityEngine.Random.Range(-0.03f, 0.03f);

        return new Vector2(Mathf.Clamp01(x), Mathf.Clamp01(y));
    }
}