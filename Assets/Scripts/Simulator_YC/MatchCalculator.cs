using System.Collections.Generic;
using UnityEngine;

public static class MatchCalculator
{
    public static float LastShootScore;
    public static float LastPassScore;
    public static float LastDribbleScore;

    public static float LastShootStat;
    public static float LastBlockPressure;

    // [기획서 3.1] 종횡비 보정값 1.87 (9:16)
    private const float ASPECT_RATIO = 1.87f;

    public static float CalculateDistance(Vector2 p1, Vector2 p2)
    {
        float dx = (p2.x - p1.x) * 1.0f;
        float dy = (p2.y - p1.y) * ASPECT_RATIO;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    // 선분과 점 사이의 최단 거리 계산 (패스 경로 차단용)
    public static float DistancePointToLineSegment(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 line = end - start;
        float len = line.magnitude;
        line.Normalize();

        Vector2 v = point - start;
        float d = Vector2.Dot(v, line);
        d = Mathf.Clamp(d, 0f, len);

        Vector2 closestPoint = start + line * d;
        return CalculateDistance(point, closestPoint);
    }

    private static MatchPlayer GetNearestPlayer(MatchPlayer target, List<MatchPlayer> enemies, out float minDistance)
    {
        MatchPlayer nearest = null;
        minDistance = float.MaxValue;
        foreach (var e in enemies)
        {
            float dist = CalculateDistance(target.LogicPosition, e.LogicPosition);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = e;
            }
        }
        return nearest;
    }

    // [기획서 6.2] 행동 결정
    public static int DecideAction(MatchPlayer player, float distToHoop, TeamTactics tactics, List<MatchPlayer> teammates, List<MatchPlayer> enemies)
    {
        // 밸런스 값 가져오기
        float penDistHoop = MatchDataProxy.Instance.GetBalance("Pen_Dist_Hoop");
        float penBlock = MatchDataProxy.Instance.GetBalance("Pen_Def_Block");
        float penSteal = MatchDataProxy.Instance.GetBalance("Pen_Def_Steal");
        float wShotBase = MatchDataProxy.Instance.GetBalance("W_Shot_Base");
        float wPassBase = MatchDataProxy.Instance.GetBalance("W_Pass_Base");
        float wDribBase = MatchDataProxy.Instance.GetBalance("W_Dribble_Base");

        float nearestEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(player, enemies, out nearestEnemyDist);

        // 슛 점수 공식
        float shootStat = (distToHoop > 0.35f) ? player.GetStat(MatchStatType.ThreePoint) : player.GetStat(MatchStatType.TwoPoint);
        float enemyBlock = (nearestEnemy != null) ? nearestEnemy.GetStat(MatchStatType.Block) : 0f;
        float wShoot = (distToHoop > 0.35f) ? tactics.bonusThreePoint : tactics.bonusTwoPoint;

        LastShootStat = shootStat;
        LastBlockPressure = enemyBlock * (1f / (nearestEnemyDist + penBlock));

        float scoreShoot = (shootStat * wShoot * wShotBase)
                         - (distToHoop * penDistHoop * wShoot)
                         - (enemyBlock * (1f / (nearestEnemyDist + penBlock)) * wShoot);

        // 패스 점수 공식
        float maxPassScore = -999f;
        foreach (var mate in teammates)
        {
            if (mate == player) continue;

            float mateNearestEnemyDist;
            GetNearestPlayer(mate, enemies, out mateNearestEnemyDist);

            int hasEnemyOnPath = 0;
            float pathEnemySteal = 0f;
            foreach (var e in enemies)
            {
                if (DistancePointToLineSegment(e.LogicPosition, player.LogicPosition, mate.LogicPosition) < 0.03f)
                {
                    hasEnemyOnPath = 1;
                    pathEnemySteal = e.GetStat(MatchStatType.Steal);
                    break;
                }
            }

            float currentPassScore = (mate.GetStat(MatchStatType.Pass) * tactics.bonusPass * wPassBase)
                                   + (mateNearestEnemyDist * penDistHoop)
                                   - (hasEnemyOnPath * pathEnemySteal * tactics.bonusPass);

            if (currentPassScore > maxPassScore) maxPassScore = currentPassScore;
        }
        float scorePass = maxPassScore;

        // 드리블 점수 공식
        float enemySteal = (nearestEnemy != null) ? nearestEnemy.GetStat(MatchStatType.Steal) : 0f;

        float scoreDribble = (player.GetStat(MatchStatType.Pass) * tactics.bonusDribble * wDribBase)
                           + (nearestEnemyDist * penDistHoop * tactics.bonusDribble)
                           + (distToHoop * penDistHoop * tactics.bonusDribble)
                           - (enemySteal * (1f / (nearestEnemyDist + penSteal)) * tactics.bonusDribble);

        scoreShoot = Mathf.Max(0, scoreShoot);
        scorePass = Mathf.Max(0, scorePass);
        scoreDribble = Mathf.Max(0, scoreDribble);

        LastShootScore = scoreShoot;
        LastPassScore = scorePass;
        LastDribbleScore = scoreDribble;

        float total = scoreShoot + scorePass + scoreDribble;
        if (total <= 0) return 1;

        float rand = Random.Range(0, total);
        if (rand < scoreShoot) return 0;
        else if (rand < scoreShoot + scorePass) return 1;
        else return 2;
    }


    // 슛 성공 확률
    public static bool CalculateShootSuccess(MatchPlayer attacker, float distance, MatchTeam attackTeam, MatchTeam defendTeam)
    {
        float shootStat = (distance > 0.35f) ? attacker.GetStat(MatchStatType.ThreePoint) : attacker.GetStat(MatchStatType.TwoPoint);

        // 반경 0.05 내 가장 가까운 수비수의 블록 스탯 적용
        float blockStat = 0f;
        float minEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(attacker, defendTeam.Roster, out minEnemyDist);
        if (nearestEnemy != null && minEnemyDist <= 0.05f)
        {
            blockStat = nearestEnemy.GetStat(MatchStatType.Block);
        }

        // 거리 페널티 (멀수록 분모 증가)
        float distPenalty = 1.0f + distance;

        float denominator = shootStat + (blockStat * distPenalty);
        if (denominator <= 0) denominator = 1f;

        float prob = (shootStat / denominator) * 100f;
        effectType targetEffect = (distance > 0.35f) ? effectType.Prob3pt : effectType.Prob2pt;
        if (distance <= 0.05f) targetEffect = effectType.ProbDunk;

        float passiveBonus = 0f;
        foreach (var p in attacker.Passives)
        {
            if (p.effectType == targetEffect)
            {
                if (CheckPassiveCondition(p, attackTeam, defendTeam))
                {
                    passiveBonus += (p.effectValue * 100f);
                }
            }
        }
        prob += passiveBonus; // 최종 확률에 패시브 더하기

        return Random.Range(0f, 100f) <= prob;
    }

    // 패스 성공 확률
    public static bool CalculatePassSuccess(MatchPlayer passer, MatchPlayer receiver, MatchTeam attackTeam, MatchTeam defendTeam, out MatchPlayer interceptor)
    {
        interceptor = null;
        MatchPlayer pathEnemy = null;

        // 최단 거리 0.03 미만인 적 탐색
        foreach (var e in defendTeam.Roster)
        {
            if (DistancePointToLineSegment(e.LogicPosition, passer.LogicPosition, receiver.LogicPosition) < 0.03f)
            {
                pathEnemy = e;
                break;
            }
        }

        if (pathEnemy == null) return true; // 방해 없으면 100% 성공

        float passStat = passer.GetStat(MatchStatType.Pass);
        float stealStat = pathEnemy.GetStat(MatchStatType.Steal);

        // 스틸 패시브 로직 추가!
        float stealPassiveBonus = 0f;
        foreach (var p in pathEnemy.Passives)
        {
            if (p.effectType == effectType.ProbSteal)
            {
                // 수비수(pathEnemy) 입장이므로 내 팀이 defendTeam, 적 팀이 attackTeam
                if (CheckPassiveCondition(p, defendTeam, attackTeam))
                {
                    stealPassiveBonus += (p.effectValue * 100f);
                }
            }
        }

        float prob = (passStat / (passStat + stealStat)) * 100f;
        prob -= stealPassiveBonus;

        bool success = Random.Range(0f, 100f) <= prob;

        if (!success) interceptor = pathEnemy;
        return success;
    }

    // 드리블 성공 확률
    public static bool CalculateDribbleSuccess(MatchPlayer dribbler, List<MatchPlayer> enemies)
    {
        float minEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(dribbler, enemies, out minEnemyDist);

        if (nearestEnemy == null || minEnemyDist > 0.1f) return true; // 주변에 없으면 성공

        float dribbleStat = dribbler.GetStat(MatchStatType.Dribble);
        float stealStat = nearestEnemy.GetStat(MatchStatType.Steal);

        float prob = (dribbleStat / (dribbleStat + stealStat)) * 100f;
        return Random.Range(0f, 100f) <= prob;
    }
    // 패시브 발동 조건을 범용적으로 검사하는 함수
    public static bool CheckPassiveCondition(Player_PassiveData p, MatchTeam myTeam, MatchTeam enemyTeam)
    {
        // 조건이 없거나 '-' 이면 상시 발동
        if (string.IsNullOrEmpty(p.triggerType) || p.triggerType == "-")
            return true;

        switch (p.triggerType)
        {
            case "BehindPoint": // 우리 팀이 특정 점수차 이상 지고 있을 때 발동
                return (enemyTeam.Score - myTeam.Score) >= p.triggerValue;

            case "OnStolen": // 상대 공을 뺏으려(스틸) 할 때 발동
                return true;

            // 추후 기획서에 따라 Stat2ptLow 등의 조건이 추가되면 여기에 case를 늘려가면 됩니다

            default:
                return true;
        }
    }

    // 리바운드 가중치 추첨
    public static MatchPlayer CalculateReboundWinner(Vector2 ballDropPos, List<MatchPlayer> allPlayers)
    {
        // 낙구 지점 반경 0.35 내의 후보 선정
        List<MatchPlayer> candidates = new List<MatchPlayer>();
        foreach (var p in allPlayers)
        {
            if (CalculateDistance(p.LogicPosition, ballDropPos) <= 0.35f)
            {
                candidates.Add(p);
            }
        }

        if (candidates.Count == 0) return allPlayers[Random.Range(0, allPlayers.Count)]; // 아무도 없으면 완전 랜덤

        // Ticket 계산 및 총합
        float totalTicket = 0f;
        List<float> tickets = new List<float>();

        for (int i = 0; i < candidates.Count; i++)
        {
            float dist = CalculateDistance(candidates[i].LogicPosition, ballDropPos);
            float ticket = candidates[i].GetStat(MatchStatType.Rebound) * (1.0f - dist);
            tickets.Add(ticket);
            totalTicket += ticket;
        }

        // 가중치 랜덤
        float rand = Random.Range(0, totalTicket);
        float current = 0f;
        for (int i = 0; i < candidates.Count; i++)
        {
            current += tickets[i];
            if (rand <= current) return candidates[i];
        }

        return candidates[0];
    }
}
