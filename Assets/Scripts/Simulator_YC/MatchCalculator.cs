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
        float dx = (p2.x - p1.x) * ASPECT_RATIO;
        float dy = (p2.y - p1.y) * 1.0f;
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
        float nearestEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(player, enemies, out nearestEnemyDist);

        // 슛 점수 공식
        float shootStat = (distToHoop > 0.35f) ? player.GetStat(MatchStatType.ThreePoint) : player.GetStat(MatchStatType.TwoPoint);
        float enemyBlock = (nearestEnemy != null) ? nearestEnemy.GetStat(MatchStatType.Block) : 0f;
        float wShoot = (distToHoop > 0.35f) ? tactics.bonusThreePoint : tactics.bonusTwoPoint;

        // 수비 압박 (0~1 사이로 정규화)
        float blockPressure = enemyBlock / 100f * (1f / (nearestEnemyDist + 0.1f));
        blockPressure = Mathf.Clamp(blockPressure, 0f, 1f); // 최대 1로 제한

        LastShootStat = shootStat;
        LastBlockPressure = enemyBlock * (1f / (nearestEnemyDist + 0.1f));


        float scoreShoot = (shootStat * wShoot)
                 - (distToHoop * 100f * wShoot)
                 - (enemyBlock * (1f / (nearestEnemyDist + 0.1f)) * wShoot);

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
                    hasEnemyOnPath = 1;                              // 경로상 적 있음
                    pathEnemySteal = e.GetStat(MatchStatType.Steal); // 해당 적의 스틸 스탯
                    break;
                }
            }

            float currentPassScore = (mate.GetStat(MatchStatType.Pass) * tactics.bonusPass)
                       + (mateNearestEnemyDist * 100f)
                       - (hasEnemyOnPath * pathEnemySteal * tactics.bonusPass);

            if (currentPassScore > maxPassScore) maxPassScore = currentPassScore;
        }
        float scorePass = maxPassScore;

        // 드리블 점수 공식
        float enemySteal = (nearestEnemy != null) ? nearestEnemy.GetStat(MatchStatType.Steal) : 0f;
        float stealPressure = enemySteal / 100f * (1f / (nearestEnemyDist + 0.1f));
        stealPressure = Mathf.Clamp(stealPressure, 0f, 1f); // 최대 1로 제한

        float scoreDribble = (player.GetStat(MatchStatType.Pass) * tactics.bonusDribble)
                   + (nearestEnemyDist * 100f * tactics.bonusDribble)
                   + (distToHoop * 100f * tactics.bonusDribble)
                   - (enemySteal * (1f / (nearestEnemyDist + 0.1f)) * tactics.bonusDribble);


        scoreShoot = Mathf.Max(0, scoreShoot);
        scorePass = Mathf.Max(0, scorePass);
        scoreDribble = Mathf.Max(0, scoreDribble);

        // 임시 디버그용
        LastShootScore = scoreShoot;
        LastPassScore = scorePass;
        LastDribbleScore = scoreDribble;

        float total = scoreShoot + scorePass + scoreDribble;
        if (total <= 0) return 1;

        float rand = Random.Range(0, total);
        if (rand < scoreShoot) return 0; // Shoot
        else if (rand < scoreShoot + scorePass) return 1; // Pass
        else return 2; // Dribble
    }

    // 슛 성공 확률
    public static bool CalculateShootSuccess(MatchPlayer attacker, float distance, List<MatchPlayer> enemies)
    {
        float shootStat = (distance > 0.35f) ? attacker.GetStat(MatchStatType.ThreePoint) : attacker.GetStat(MatchStatType.TwoPoint);

        // 반경 0.05 내 가장 가까운 수비수의 블록 스탯 적용
        float blockStat = 0f;
        float minEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(attacker, enemies, out minEnemyDist);
        if (nearestEnemy != null && minEnemyDist <= 0.05f)
        {
            blockStat = nearestEnemy.GetStat(MatchStatType.Block);
        }

        // 거리 페널티 (멀수록 분모 증가)
        float distPenalty = 1.0f + distance;

        float denominator = shootStat + (blockStat * distPenalty);
        if (denominator <= 0) denominator = 1f;

        float prob = (shootStat / denominator) * 100f;

        return Random.Range(0f, 100f) <= prob;
    }

    // 패스 성공 확률
    public static bool CalculatePassSuccess(MatchPlayer passer, MatchPlayer receiver, List<MatchPlayer> enemies, out MatchPlayer interceptor)
    {
        interceptor = null;
        MatchPlayer pathEnemy = null;

        // 최단 거리 0.03 미만인 적 탐색
        foreach (var e in enemies)
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

        float prob = (passStat / (passStat + stealStat)) * 100f;
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
