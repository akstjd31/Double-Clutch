using System.Collections.Generic;
using UnityEngine;

public static class MatchCalculator
{
    public static float LastShootScore;
    public static float LastPassScore;
    public static float LastDribbleScore;

    public static float LastShootStat;
    public static float LastBlockPressure;

    // [기획서 3.1] 종횡비 보정값 1.87 (16:9)
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
        minDistance = 10f;

        if (enemies == null || enemies.Count == 0)
            return null;
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
        if (nearest == null) minDistance = 10f;

        return nearest;
    }

    // [기획서 6.2] 행동 결정
    public static int DecideAction(MatchPlayer player, float distToHoop, TeamTactics tactics, List<MatchPlayer> teammates, List<MatchPlayer> enemies, float interceptDist)
    {
        // 밸런스 값 가져오기
        float penDistHoop = MatchDataProxy.Instance.GetBalance("Pen_Dist_Hoop");
        float penBlock = MatchDataProxy.Instance.GetBalance("Pen_Def_Block");
        float penSteal = MatchDataProxy.Instance.GetBalance("Pen_Def_Steal");
        float wShotBase = MatchDataProxy.Instance.GetBalance("W_Shot_Base");
        float wPassBase = MatchDataProxy.Instance.GetBalance("W_Pass_Base");
        float wDribBase = MatchDataProxy.Instance.GetBalance("W_Dribble_Base");
        float minShootScore = MatchDataProxy.Instance.GetBalance("Min_Shoot_Score");

        float nearestEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(player, enemies, out nearestEnemyDist);

        // 슛 점수 공식
        float shootStat = (distToHoop > 0.35f) ? player.GetStat(MatchStatType.ThreePoint) : player.GetStat(MatchStatType.TwoPoint);
        float enemyBlock = (nearestEnemy != null) ? nearestEnemy.GetStat(MatchStatType.Block) : 0f;
        float wShoot = (distToHoop > 0.35f) ? tactics.bonusThreePoint : tactics.bonusTwoPoint;

        LastShootStat = shootStat;
        LastBlockPressure = enemyBlock * (1f / (nearestEnemyDist + penBlock));

        float scoreShoot = (shootStat * wShoot * wShotBase)
                         + (100f / (distToHoop + 1f))
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
                if (DistancePointToLineSegment(e.LogicPosition, player.LogicPosition, mate.LogicPosition) < interceptDist)
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

        if (distToHoop > 0.84f)
        {
            scoreShoot = -999f; // 거리가 0.8415(하프라인) 밖이면 슛 점수를 마이너스로 고정해 절대 안 쏘게 만듦 ( 역습이나 공수전환중에 노마크 발생으로 인한 확정 골 오류 방지 )
        }
        else
        {
            scoreShoot = Mathf.Max(minShootScore, scoreShoot);
        }
        scorePass = Mathf.Max(0, scorePass);
        scoreDribble = Mathf.Max(0, scoreDribble);

        LastShootScore = scoreShoot;
        LastPassScore = scorePass;
        LastDribbleScore = scoreDribble;

        float total = scoreShoot + scorePass + scoreDribble;
        int chosenAction = 1;
        if (total <= 0)
        {
            chosenAction = 1;
        }
        else
        {
            float rand = Random.Range(0, total);
            if (rand < scoreShoot) chosenAction = 0;
            else if (rand < scoreShoot + scorePass) chosenAction = 1;
            else chosenAction = 2;
        }

        string actionName = chosenAction == 0 ? "슛" : (chosenAction == 1 ? "패스" : "드리블");

        string playerName = MakeName(player.PlayerName);

        // 기획서 원본 공식이 그대로 보이는 행동 결정 디버그 로그!
        Debug.Log($"<color=#FFFF00>[행동 결정 디버그]</color> {playerName} (골대거리:{distToHoop:F2}, 수비거리:{nearestEnemyDist:F2})\n" +
                  $"▶ 슛 공식: ({shootStat}*{wShoot}*{wShotBase}) + (100/({distToHoop:F2}+1)) - ({enemyBlock}*(1/({nearestEnemyDist:F2}+{penBlock}))*{wShoot})\n" +
                  $"▶ 결과 점수 => 슛: {scoreShoot:F2} | 패스: {scorePass:F2} | 드리블: {scoreDribble:F2}\n" +
                  $"▶ <color=#00FF00>최종 AI 선택: {actionName}</color>");

        return chosenAction;
    }


    // 슛 성공 확률
    public static bool CalculateShootSuccess(MatchPlayer attacker, float distance, MatchTeam attackTeam, MatchTeam defendTeam, TeamTactics attackTactics, TeamTactics defendTactics, float blockDist)
    {
        float shootStat = (distance > 0.35f) ? attacker.GetStat(MatchStatType.ThreePoint, attackTactics.bonusThreePoint) : attacker.GetStat(MatchStatType.TwoPoint, attackTactics.bonusTwoPoint);

        float blockStat = 0f;
        float minEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(attacker, defendTeam.Roster, out minEnemyDist);

        if (nearestEnemy != null && minEnemyDist <= blockDist)
        {
            blockStat = nearestEnemy.GetStat(MatchStatType.Block, defendTactics.bonusBlock);
        }
        Debug.Log($"[수비 체크] 공격수 위치: {attacker.LogicPosition} | 수비수 위치: {nearestEnemy.LogicPosition} | 최단거리: {minEnemyDist:F4} | 수비발동?: {minEnemyDist <= blockDist}");

        float penDistHoop = MatchDataProxy.Instance.GetBalance("Pen_Dist_Hoop");
        float distancePenalty = 1f + (distance * penDistHoop);

        // 공식: { 33 + (공격수 슛 * 0.67) / (100 + 적 블록 * 거리 페널티) } * 100
        float calcStat = (shootStat * 0.67f) / (100f + blockStat * distancePenalty);
        float prob = 33f + (calcStat * 100f);


        effectType targetEffect = (distance > 0.35f) ? effectType.Prob3pt : effectType.Prob2pt;
        if (distance <= 0.05f) targetEffect = effectType.ProbDunk;

        float passiveBonus = 0f;
        foreach (var p in attacker.Passives)
        {
            if (p.effectType == targetEffect)
            {
                passiveBonus += (p.effectValue * 100f);
            }
        }
        prob += passiveBonus; // 최종 확률에 패시브 더하기
        float dice = Random.Range(0f, 100f);
        bool isSuccess = dice <= prob;

        string attackerName = MakeName(attacker.PlayerName);
        string eName = nearestEnemy != null ? MakeName(nearestEnemy.PlayerName) : "없음";
        Debug.Log($"<color=#FF8C00>[슛 디버그]</color> {attackerName} 슛 시도 (골대거리:{distance:F2})\n" +
              $"▶ 공격 슛스탯: {shootStat} | 수비({eName}) 블록스탯: {blockStat} (수비거리:{minEnemyDist:F2})\n" +
              $"▶ 공식: 33 + ( ({shootStat} * 0.67) / (100 + {blockStat} * {distancePenalty:F2}) ) * 100\n" +
              $"▶ 적용: 33 + {calcStat * 100f:F2} + 패시브({passiveBonus}%) = {prob:F2}%\n" +
              $"▶ <color=#00FF00>최종확률: {prob:F2}%</color> | 주사위: {dice:F2} => {(isSuccess ? "<b>골!</b>" : "<b>노골</b>")}");

        return isSuccess;
    }

    // 패스 성공 확률
    public static bool CalculatePassSuccess(MatchPlayer passer, MatchPlayer receiver, MatchTeam attackTeam, MatchTeam defendTeam, TeamTactics attackTactics, TeamTactics defendTactics, float interceptDist, out MatchPlayer interceptor)
    {
        interceptor = null;
        MatchPlayer pathEnemy = null;

        // 최단 거리 0.03 미만 (interceptDist)인 적 탐색
        foreach (var e in defendTeam.Roster)
        {
            if (DistancePointToLineSegment(e.LogicPosition, passer.LogicPosition, receiver.LogicPosition) < interceptDist)
            {
                pathEnemy = e;
                break;
            }
        }

        if (pathEnemy == null)
        {
            Debug.Log($"<color=#00BFFF>[패스 디버그]</color> {passer.PlayerName} -> {receiver.PlayerName}\n" +
                      $"▶ 패스 경로에 방해하는 적이 없습니다. (100% 안전한 패스 성공)");
            return true;
        }

        float passStat = passer.GetStat(MatchStatType.Pass, attackTactics.bonusPass);
        float stealStat = pathEnemy.GetStat(MatchStatType.Steal, defendTactics.bonusSteal);

        // 스틸 패시브 로직 추가
        float stealPassiveBonus = 0f;
        foreach (var p in pathEnemy.Passives)
        {
            // 적 수비수의 패시브가 스틸 확률 증가라면 무조건 발동
            if (p.effectType == effectType.ProbSteal)
            {
                stealPassiveBonus += (p.effectValue * 100f);
            }
        }

        float prob = (passStat / (passStat + stealStat)) * 100f;
        prob -= stealPassiveBonus;


        float dice = Random.Range(0f, 100f);
        bool success = dice <= prob;

        Debug.Log($"<color=#00BFFF>패스 디버그</color> {passer.PlayerName}->{receiver.PlayerName} (차단시도:{pathEnemy.PlayerName})\n" +
                  $"▶ 공격 패스스탯: {passStat} | 수비 스틸스탯: {stealStat} (차단판정거리: {interceptDist:F2})\n" +
                  $"▶ 공식: (({passStat} / {passStat + stealStat}) * 100) - 수비패시브({stealPassiveBonus}%) = {prob:F2}%\n" +
                  $"▶ <color=#00FF00>최종확률: {prob:F2}%</color> | 주사위: {dice:F2} => {(success ? "<b>성공</b>" : "<b>차단당함</b>")}");

        if (!success) interceptor = pathEnemy;
        return success;
    }

    // 드리블 성공 확률
    public static bool CalculateDribbleSuccess(MatchPlayer dribbler, List<MatchPlayer> enemies, TeamTactics attackTactics, TeamTactics defendTactics, float dribbleBlockDist)
    {
        float minEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(dribbler, enemies, out minEnemyDist);

        string dribblerName = MakeName(dribbler.PlayerName);

        if (nearestEnemy == null)
        {
            Debug.Log($"<color=#DA70D6>[드리블 디버그]</color> {dribblerName} 드리블 이동\n" +
                      $"▶ 코트 위에 매치업 대상(수비수)이 존재하지 않습니다!\n" +
                      $"▶ 방해 없음! 100% 안전한 드리블 성공");
            return true;
        }

        string enemyName = MakeName(nearestEnemy.PlayerName);

        if (minEnemyDist > dribbleBlockDist)
        {
            Debug.Log($"<color=#DA70D6>[드리블 디버그]</color> {dribblerName} 노마크 돌파\n" +
                      $"▶ 수비수({enemyName})와의 최단거리가 {dribbleBlockDist} 밖입니다. (거리:{minEnemyDist:F2})\n" +
                      $"▶ 방해 없음! 100% 안전한 드리블 성공");
            return true;
        }

        float dribbleStat = dribbler.GetStat(MatchStatType.Pass, attackTactics.bonusDribble);
        float stealStat = nearestEnemy.GetStat(MatchStatType.Steal, defendTactics.bonusSteal);

        // 밸런스 가중치 적용
        float wStealBalance = MatchDataProxy.Instance.GetBalance("W_Def_Steal_Dribble");
        if (wStealBalance <= 0f) wStealBalance = 1.0f;

        // 수비수 스틸 패시브 적용
        float stealPassiveBonus = 0f;
        foreach (var p in nearestEnemy.Passives)
        {
            if (p.effectType == effectType.ProbSteal)
            {
                stealPassiveBonus += (p.effectValue * 100f);
            }
        }

        float weightedStealStat = stealStat * wStealBalance;
        float prob = (dribbleStat / (dribbleStat + stealStat)) * 100f;
        prob -= stealPassiveBonus;

        float dice = Random.Range(0f, 100f);
        bool success = dice <= prob;

        Debug.Log($"<color=#DA70D6>[드리블 디버그]</color> {dribblerName} 돌파 경합! (수비수: {enemyName}, 거리:{minEnemyDist:F2})\n" +
                  $"▶ 내 드리블 스탯: {dribbleStat} | 적 스틸 스탯: {stealStat} (가중치 {wStealBalance}배 적용 -> {weightedStealStat})\n" +
                  $"▶ <color=#00FF00>최종 돌파 확률: {prob:F2}%</color> | 주사위: {dice:F2} => {(success ? "<b>돌파 성공(전진)!</b>" : "<b>수비에 막힘(좌우이동)</b>")}");

        return success;

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

    private static string MakeName(string[] nameKey)
    {
        StringManager manager = StringManager.Instance;
        string name = manager.GetString(nameKey[0]) + manager.GetString(nameKey[1]) + manager.GetString(nameKey[2]);
        return name;
    }
}
