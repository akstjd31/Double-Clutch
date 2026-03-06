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
    public static int DecideAction(MatchPlayer player, float distToHoop, TeamTactics tactics, List<MatchPlayer> teammates, List<MatchPlayer> enemies)
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
                         - (distToHoop * penDistHoop * wShoot)
                         - (enemyBlock * (1f / (nearestEnemyDist + penBlock)) * wShoot);

        // 패스 점수 공식

        float interceptDist = MatchDataProxy.Instance.GetBalance("Pen_Intercept_Dist");
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

        if (distToHoop > 0.5f)
        {
            scoreShoot = -999f; // 거리가 0.5(하프라인) 밖이면 슛 점수를 마이너스로 고정해 절대 안 쏘게 만듦 ( 역습이나 공수전환중에 노마크 발생으로 인한 확정 골 오류 방지 )
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
        if (total <= 0) return 1;

        float rand = Random.Range(0, total);
        if (rand < scoreShoot) return 0;
        else if (rand < scoreShoot + scorePass) return 1;
        else return 2;
    }


    // 슛 성공 확률
    public static bool CalculateShootSuccess(MatchPlayer attacker, float distance, MatchTeam attackTeam, MatchTeam defendTeam, TeamTactics attackTactics, TeamTactics defendTactics)
    {
        float shootStat = (distance > 0.35f) ? attacker.GetStat(MatchStatType.ThreePoint, attackTactics.bonusThreePoint) : attacker.GetStat(MatchStatType.TwoPoint, attackTactics.bonusTwoPoint);

        // 반경 0.05 내 가장 가까운 수비수의 블록 스탯 적용
        float blockDist = MatchDataProxy.Instance.GetBalance("Def_Block_Dist");
        if (blockDist <= 0f) blockDist = 0.15f;
        float blockStat = 0f;
        float minEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(attacker, defendTeam.Roster, out minEnemyDist);
        if (nearestEnemy != null && minEnemyDist <= blockDist)
        {
            blockStat = nearestEnemy.GetStat(MatchStatType.Block, defendTactics.bonusBlock);
        }
        Debug.Log($"[수비 체크] 공격수 위치: {attacker.LogicPosition} | 수비수 위치: {nearestEnemy.LogicPosition} | 최단거리: {minEnemyDist:F4} | 수비발동?: {minEnemyDist <= blockDist}");

        float denominator = shootStat + blockStat;
        if (denominator <= 0) denominator = 1f;

        float prob = (shootStat / denominator) * 100f;

        float penDistHoop = MatchDataProxy.Instance.GetBalance("Pen_Dist_Hoop");
        prob -= (distance * penDistHoop);


        effectType targetEffect = (distance > 0.35f) ? effectType.Prob3pt : effectType.Prob2pt;
        if (distance <= 0.05f) targetEffect = effectType.ProbDunk;

        float passiveBonus = 0f;
        //foreach (var p in attacker.Passives)
        //{
        //    if (p.effectType == targetEffect)
        //    {
        //        if (CheckPassiveCondition(p, attackTeam, defendTeam))
        //        {
        //            passiveBonus += (p.effectValue * 100f);
        //        }
        //    }
        //}
        prob += passiveBonus; // 최종 확률에 패시브 더하기
        float dice = Random.Range(0f, 100f);
        bool isSuccess = dice <= prob;

        string eName = nearestEnemy != null ? nearestEnemy.PlayerName : "없음";
        Debug.Log($"<color=#FF8C00>[슛 디버그]</color> {attacker.PlayerName} 슛 시도 (거리:{distance:F2})\n" +
                  $"▶ 공격 슛스탯: {shootStat} | 수비({eName}) 블록스탯: {blockStat} (거리:{minEnemyDist:F2})\n" +
                  $"▶ 공식: ({shootStat} / {denominator:F2}) * 100 - ({distance:F2} * {penDistHoop}) = {prob - passiveBonus:F2}% | 패시브: +{passiveBonus}%\n" +
                  $"▶ <color=#00FF00>최종확률: {prob:F2}%</color> | 주사위: {dice:F2} => {(isSuccess ? "<b>골!</b>" : "<b>노골</b>")}");

        return isSuccess;
    }

    // 패스 성공 확률
    public static bool CalculatePassSuccess(MatchPlayer passer, MatchPlayer receiver, MatchTeam attackTeam, MatchTeam defendTeam, TeamTactics attackTactics, TeamTactics defendTactics, out MatchPlayer interceptor)
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

        float passStat = passer.GetStat(MatchStatType.Pass, attackTactics.bonusPass);
        float stealStat = pathEnemy.GetStat(MatchStatType.Steal, defendTactics.bonusSteal);

        // 스틸 패시브 로직 추가!
        float stealPassiveBonus = 0f;
        //foreach (var p in pathEnemy.Passives)
        //{
        //    if (p.effectType == effectType.ProbSteal)
        //    {
        //        // 수비수(pathEnemy) 입장이므로 내 팀이 defendTeam, 적 팀이 attackTeam
        //        if (CheckPassiveCondition(p, defendTeam, attackTeam))
        //        {
        //            stealPassiveBonus += (p.effectValue * 100f);
        //        }
        //    }
        //}

        float prob = (passStat / (passStat + stealStat)) * 100f;
        prob -= stealPassiveBonus;


        float dice = Random.Range(0f, 100f);
        bool success = dice <= prob;

        Debug.Log($"<color=#00BFFF>패스 디버그</color> {passer.PlayerName}->{receiver.PlayerName} (차단시도:{pathEnemy.PlayerName})\n" +
                  $"▶ 공격 패스스탯: {passStat} | 수비 스틸스탯: {stealStat}\n" +
                  $"▶ 공식: ({passStat} / {passStat + stealStat}) * 100 = {prob + stealPassiveBonus:F2}% | 수비패시브: -{stealPassiveBonus}%\n" +
                  $"▶ <color=#00FF00>최종확률: {prob:F2}%</color> | 주사위: {dice:F2} => {(success ? "<b>성공</b>" : "<b>차단당함</b>")}");

        if (!success) interceptor = pathEnemy;
        return success;
    }

    // 드리블 성공 확률
    public static bool CalculateDribbleSuccess(MatchPlayer dribbler, List<MatchPlayer> enemies, TeamTactics attackTactics, TeamTactics defendTactics)
    {
        float minEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(dribbler, enemies, out minEnemyDist);

        if (nearestEnemy == null || minEnemyDist > 0.1f) return true; // 주변에 없으면 성공

        float dribbleStat = dribbler.GetStat(MatchStatType.Pass, attackTactics.bonusDribble);
        float stealStat = nearestEnemy.GetStat(MatchStatType.Steal, defendTactics.bonusSteal);

        float prob = (dribbleStat / (dribbleStat + stealStat)) * 100f;
        float dice = Random.Range(0f, 100f);
        bool success = dice <= prob;

        Debug.Log($"<color=#DA70D6>드리블 디버그</color> {dribbler.PlayerName} (수비수:{nearestEnemy.PlayerName}, 거리:{minEnemyDist:F2})\n" +
                  $"▶ 공격 드리블(패스)스탯: {dribbleStat} | 수비 스틸스탯: {stealStat}\n" +
                  $"▶ <color=#00FF00>최종확률: ({dribbleStat}/{dribbleStat + stealStat})*100 = {prob:F2}%</color> | 주사위: {dice:F2} => {(success ? "<b>성공</b>" : "<b>스틸당함</b>")}");

        return success;

    }
    // 패시브 발동 조건을 범용적으로 검사하는 함수 (triggerCond,triggerValue가 사라짐)
    //public static bool CheckPassiveCondition(Player_PassiveData p, MatchTeam myTeam, MatchTeam enemyTeam)
    //{
    //    // triggerCond는 Enum이므로 None일 때 상시 발동으로 처리합니다.
    //    if (p.triggerCond == triggerCond.None)
    //        return true;

    //    switch (p.triggerCond)
    //    {
    //        case triggerCond.ScoreGap: // 우리 팀이 특정 점수차 이상 지고 있을 때 발동
    //            return (enemyTeam.SimulatedScore - myTeam.SimulatedScore) >= p.triggerValue;

    //        case triggerCond.Random: // 만약 0보다 큰 값이 들어온다면 해당 값을 확률(%)로 취급
    //            if (p.triggerValue == 0) return true;
    //            return UnityEngine.Random.Range(0, 100) < p.triggerValue;

    //        case triggerCond.ReboundDiff: // 리바운드가 특정 수치 이상 밀릴 때
    //            return (myTeam.ReboundCount - enemyTeam.ReboundCount) <= p.triggerValue;

    //        case triggerCond.Stat2ptLow: // 2점슛 성공률이 낮을 때
    //            float pt2Rate = myTeam.Try2pt == 0 ? 0 : ((float)myTeam.Succ2pt / myTeam.Try2pt) * 100f;
    //            return myTeam.Try2pt > 0 && pt2Rate <= p.triggerValue;

    //        case triggerCond.Stat3ptLow: // 3점슛 성공률이 낮을 때
    //            float pt3Rate = myTeam.Try3pt == 0 ? 0 : ((float)myTeam.Succ3pt / myTeam.Try3pt) * 100f;
    //            return myTeam.Try3pt > 0 && pt3Rate <= p.triggerValue;

    //        default:
    //            return true;
    //    }
    //}

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
