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

    // 팀의 활성화된 시너지 합산을 가져오는 헬퍼 함수
    public static float GetSynergyBonus(MatchTeam homeTeam, effectType type)
    {
        if (homeTeam == null || homeTeam.ActiveSynergies == null) return 0f;
        float bonus = 0f;
        foreach (var syn in homeTeam.ActiveSynergies)
        {
            if (syn.effectType == type) bonus += syn.effectValue;
        }
        return bonus;
    }
    public static float GetMaxSynergyBonus(MatchTeam homeTeam, effectType type)
    {
        if (homeTeam == null || homeTeam.ActiveSynergies == null) return 0f;
        float maxBonus = 0f;
        foreach (var syn in homeTeam.ActiveSynergies)
        {
            // 합산(+=)하지 않고, 기존 값보다 크면 덮어씌움 (최댓값만 추출)
            if (syn.effectType == type && syn.effectValue > maxBonus)
            {
                maxBonus = syn.effectValue;
            }
        }
        return maxBonus;
    }
    // '모든 능력치' 증감 시너지 3종 일괄 합산 함수
    public static float GetAllStatBonus(MatchTeam homeTeam)
    {
        if (homeTeam == null) return 0f;
        return GetSynergyBonus(homeTeam, effectType.AbsoluteTrust)
             - GetSynergyBonus(homeTeam, effectType.Discordance)
             - GetSynergyBonus(homeTeam, effectType.DifferentDreams);
    }
    // 스탯을 가져올 때 자동으로 '모든 능력치' 시너지를 발라주는 래퍼 함수 (기본)
    public static float GetPlayerStat(MatchPlayer player, MatchStatType statType, MatchTeam homeTeam)
    {
        float stat = player.GetStat(statType);
        if (homeTeam != null && homeTeam.Roster.Contains(player)) stat += GetAllStatBonus(homeTeam);
        return Mathf.Max(0, stat);
    }

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
    public static int DecideAction(MatchPlayer player, float distToHoop, TeamTactics tactics, MatchTeam attackTeam, MatchTeam defendTeam, float interceptDist, MatchTeam homeTeam)
    {
        bool isHomeAttacking = homeTeam == attackTeam;
        // 밸런스 값 가져오기
        float homePassBonus = GetSynergyBonus(homeTeam, effectType.SystemBasket);
        float homeBlockBonus = GetSynergyBonus(homeTeam, effectType.SuffocatingDefense);
        float enemyStealDebuff = GetSynergyBonus(homeTeam, effectType.AnkleBreaker);

        float wShotBase = MatchDataProxy.Instance.GetBalance("W_Shot_Base");
        float wPassBase = MatchDataProxy.Instance.GetBalance("W_Pass_Base");
        float wDribBase = MatchDataProxy.Instance.GetBalance("W_Dribble_Base");

        float penDistHoop = MatchDataProxy.Instance.GetBalance("Pen_Dist_Hoop");
        float penBlock = MatchDataProxy.Instance.GetBalance("Pen_Def_Block");
        float penSteal = MatchDataProxy.Instance.GetBalance("Pen_Def_Steal");

        float wBlockBase = MatchDataProxy.Instance.GetBalance("W_Block_Base");
        float wStealBase = MatchDataProxy.Instance.GetBalance("W_Steal_Base");
        float wDribbleBonus = MatchDataProxy.Instance.GetBalance("W_Dribble_Bonus");
        float wDistBonus = MatchDataProxy.Instance.GetBalance("W_Dist_Bonus");
        float wDefault = MatchDataProxy.Instance.GetBalance("W_Default");
        float minShootScore = MatchDataProxy.Instance.GetBalance("Min_Shoot_Score");


        float nearestEnemyDist;
        MatchPlayer nearestEnemy = GetNearestPlayer(player, defendTeam.Roster, out nearestEnemyDist);

        // 슛 점수 공식
        // [블락 적용] 아군 수비(적군 슛)일 때 아군의 질식수비 보너스 적용
        float blockStat = (nearestEnemy != null) ? GetPlayerStat(nearestEnemy, MatchStatType.Block, homeTeam) : 0f;
        if (!isHomeAttacking && nearestEnemy != null) blockStat += homeBlockBonus;

        float shootStat = (distToHoop > 0.35f) ? GetPlayerStat(player, MatchStatType.ThreePoint, homeTeam) : GetPlayerStat(player, MatchStatType.TwoPoint, homeTeam);
        float wShoot = (distToHoop > 0.35f) ? tactics.bonusThreePoint : tactics.bonusTwoPoint;

        LastShootStat = shootStat;
        LastBlockPressure = blockStat * (1f / (nearestEnemyDist + penBlock));

        float scoreShoot = (shootStat * wShoot * wShotBase)
                         + (100f / (distToHoop + 1f))
                         - (blockStat * (1f / (nearestEnemyDist + penBlock)) * wBlockBase);


        // 특수 조건 시너지를 AI 슛 판단 점수에 반영
        float aiBonusScore = 0f;
        string activeSynergyLog = ""; // 디버그 로그용

        if (isHomeAttacking)
        {
            // 트랜지션 마스터 (패스를 받은 선수의 2점 슛 점수 로직 + effectValue)
            // 엔진에서 방금 막 패스를 받았으면 4틱을 부여하므로, 틱이 '3'일 때가 바로 직후 1틱입니다.
            if (distToHoop <= 0.35f && player.PassReceivedBuffTick == 3)
            {
                aiBonusScore += GetSynergyBonus(homeTeam, effectType.TransitionMaster);
                activeSynergyLog += "[트랜지션마스터] ";
            }
        }
        scoreShoot += aiBonusScore;

        // 패스 점수 공식

        float maxPassScore = -999f;
        foreach (var mate in attackTeam.Roster)
        {
            if (mate == player) continue;

            float mateNearestEnemyDist;
            GetNearestPlayer(mate, defendTeam.Roster, out mateNearestEnemyDist);

            int hasEnemyOnPath = 0;
            float pathEnemySteal = 0f;
            float closestDist = float.MaxValue;
            foreach (var e in defendTeam.Roster)
            {
                if (DistancePointToLineSegment(e.LogicPosition, player.LogicPosition, mate.LogicPosition) < interceptDist)
                {
                    float distToPasser = CalculateDistance(player.LogicPosition, e.LogicPosition);
                    if (distToPasser < closestDist)
                    {
                        closestDist = distToPasser;
                        hasEnemyOnPath = 1;
                        // [스틸 디버프 적용] 아군 공격 시, 길목에 있는 적군의 스틸 능력치 감소 (앵클 브레이커)
                        float eSteal = GetPlayerStat(e, MatchStatType.Steal, homeTeam);
                        if (isHomeAttacking) eSteal = Mathf.Max(0, eSteal - enemyStealDebuff);
                        pathEnemySteal = eSteal;
                    }
                }
            }
            // [패스 보너스 적용] 아군 공격 시에만 아군에게 시스템 바스켓 적용
            float matePassStat = GetPlayerStat(mate, MatchStatType.Pass, homeTeam);
            if (isHomeAttacking) matePassStat += homePassBonus;
            float currentPassScore = (matePassStat * tactics.bonusPass * wPassBase)
                                   + (mateNearestEnemyDist * wDistBonus)
                                   - (hasEnemyOnPath * pathEnemySteal * wStealBase);

            if (currentPassScore > maxPassScore) maxPassScore = currentPassScore;
        }
        float scorePass = maxPassScore;

        // 드리블 점수 공식
        float dribblerPassStat = GetPlayerStat(player, MatchStatType.Pass, homeTeam);
        if (isHomeAttacking) dribblerPassStat += homePassBonus; // 드리블(패스기반) 상승

        float enemySteal = (nearestEnemy != null) ? GetPlayerStat(nearestEnemy, MatchStatType.Steal, homeTeam) : 0f;
        if (isHomeAttacking && nearestEnemy != null) enemySteal = Mathf.Max(0, enemySteal - enemyStealDebuff); // 적 스틸 감소

        float scoreDribble = (dribblerPassStat * tactics.bonusDribble * wDribBase)
                           + (nearestEnemyDist * wDribbleBonus)
                           + (distToHoop * wDistBonus)
                           - (enemySteal * (1f / (nearestEnemyDist + penSteal)) * wStealBase);

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
                  $"▶ 슛 공식: ({shootStat:F1}*{wShoot:F1}*{wShotBase}) + (100/({distToHoop:F2}+1)) - ({blockStat:F1}*(1/({nearestEnemyDist:F2}+{penBlock}))*{wBlockBase}) = {scoreShoot:F2}\n" +
                  $"▶ 드리블 공식: ({dribblerPassStat:F1}*{tactics.bonusDribble:F1}*{wDribBase}) + ({nearestEnemyDist:F2}*{wDribbleBonus}) + ({distToHoop:F2}*{wDistBonus}) - ({enemySteal:F1}*(1/({nearestEnemyDist:F2}+{penSteal}))*{wStealBase}) = {scoreDribble:F2}\n" +
                  $"▶ 패스 점수: 최고 효율 대상 탐색 결과 = {scorePass:F2}\n" +
                  $"▶ 발동된 판단 시너지: {(string.IsNullOrEmpty(activeSynergyLog) ? "없음" : activeSynergyLog)}\n" +
                  $"▶ <color=#00FF00>최종 AI 선택: {actionName}</color> (슛:{scoreShoot:F1} / 패스:{scorePass:F1} / 드리블:{scoreDribble:F1})");

        return chosenAction;
    }


    // 슛 성공 확률
    public static bool CalculateShootSuccess(MatchPlayer attacker, float distance, MatchTeam homeTeam, MatchTeam awayTeam, TeamTactics homeTactics, TeamTactics awayTactics, float blockDist)
    {

        bool isHomeAttacking = homeTeam.Roster.Contains(attacker);
        TeamTactics myTactics = isHomeAttacking ? homeTactics : awayTactics;
        TeamTactics enemyTactics = isHomeAttacking ? awayTactics : homeTactics;
        List<MatchPlayer> enemies = isHomeAttacking ? awayTeam.Roster : homeTeam.Roster;

        float shootStat = (distance > 0.35f) ? GetPlayerStat(attacker, MatchStatType.ThreePoint, homeTeam) : GetPlayerStat(attacker, MatchStatType.TwoPoint, homeTeam);

        shootStat *= (distance > 0.35f) ? myTactics.bonusThreePoint : myTactics.bonusTwoPoint;

        float blockStat = 0f;

        MatchPlayer nearestEnemy = GetNearestPlayer(attacker, enemies, out float minEnemyDist);

        if (nearestEnemy != null && minEnemyDist <= blockDist)
        {
            blockStat = GetPlayerStat(nearestEnemy, MatchStatType.Block, homeTeam);
            // [시너지] 적군 공격(내가 수비)일 때 내 블락 스탯 증가 (질식 수비)
            if (!isHomeAttacking) blockStat += GetSynergyBonus(homeTeam, effectType.SuffocatingDefense);
            // 가장 마지막에 전술 배율 곱하기
            blockStat *= enemyTactics.bonusBlock;
        }



        Debug.Log($"[수비 체크] 공격수 위치: {attacker.LogicPosition} | 수비수 위치: {nearestEnemy.LogicPosition} | 최단거리: {minEnemyDist:F4} | 수비발동?: {minEnemyDist <= blockDist}");

        float penDistHoop = MatchDataProxy.Instance.GetBalance("Pen_Dist_Hoop");
        float distancePenalty = 1f + (distance * penDistHoop);

        // 공식: { 33 + (공격수 슛 * 0.67) / (100 + 적 블록 * 거리 페널티) } * 100
        float calcStat = (shootStat * 0.67f) / (100f + blockStat * distancePenalty);
        float prob = 33f + (calcStat * 100f);

        float extraBonus = 0f;
        string activeSynergyLog = ""; // 디버그 로그용

        if (isHomeAttacking)
        {
            int scoreGap = homeTeam.SimulatedScore - awayTeam.SimulatedScore; // 내 점수 - 적 점수

            // 승부사의 심장 (10점 차 이상 지고 있을 때 3점슛 확률 증가)
            if (distance > 0.35f && scoreGap <= -10)
            {
                extraBonus += GetSynergyBonus(homeTeam, effectType.ClutchHeart); // 확률이므로 100을 곱해 % 단위로 맞춤
                activeSynergyLog += "[승부사의심장] ";
            }

            // 고릴라 덩크 (거리 0.01 ~ 0.05 사이일 때 슛 성공률 증가 - 패스 무관)
            if (distance >= 0.01f && distance <= 0.05f)
            {
                extraBonus += GetSynergyBonus(homeTeam, effectType.GorillaDunk);
                activeSynergyLog += "[고릴라덩크] ";
            }

            // 패스 연계 시너지 (패스받은 직후 3틱 이내일 때만 발동)
            if (attacker.PassReceivedBuffTick > 0)
            {
                if (distance > 0.35f) // 3점 (스페이스 오퍼레이터)
                {
                    extraBonus += GetMaxSynergyBonus(homeTeam, effectType.SpaceOperator);
                    activeSynergyLog += "[스페이스오퍼레이터] ";
                }
                else  // 2점 (컷인 플레이)
                {
                    extraBonus += GetMaxSynergyBonus(homeTeam, effectType.CutInPlay);
                    activeSynergyLog += "[컷인플레이] ";
                }
                
            }
        }
        prob += extraBonus;
        float dice = Random.Range(0f, 100f);
        bool isSuccess = dice <= prob;

        string attackerName = MakeName(attacker.PlayerName);
        string eName = nearestEnemy != null ? MakeName(nearestEnemy.PlayerName) : "없음";
        Debug.Log($"<color=#FF8C00>[슛 디버그]</color> {attackerName} 슛 시도 (골대거리:{distance:F2})\n" +
              $"▶ 공격 슛스탯: {shootStat} | 수비({eName}) 블록스탯: {blockStat} (수비거리:{minEnemyDist:F2})\n" +
              $"▶ 발동된 확률 추가 시너지: {(string.IsNullOrEmpty(activeSynergyLog) ? "없음" : activeSynergyLog)}\n" +
              $"▶ 공식: 33 + ( ({shootStat} * 0.67) / (100 + {blockStat} * {distancePenalty:F2}) ) * 100\n" +
              $"▶ 적용: 33 + {calcStat * 100f:F2} + 추가확률보정({extraBonus:F4}%) = {prob:F4}%\n" +
              $"▶ <color=#00FF00>최종확률: {prob:F4}%</color> | 주사위: {dice:F4} => {(isSuccess ? "<b>골!</b>" : "<b>노골</b>")}");

        return isSuccess;
    }

    // 패스 성공 확률
    public static bool CalculatePassSuccess(MatchPlayer passer, MatchPlayer receiver, MatchTeam homeTeam, MatchTeam awayTeam, TeamTactics homeTactics, TeamTactics awayTactics, float interceptDist, out MatchPlayer interceptor)
    {
        interceptor = null;
        MatchPlayer pathEnemy = null;

        bool isHomeAttacking = homeTeam.Roster.Contains(passer);
        TeamTactics myTactics = isHomeAttacking ? homeTactics : awayTactics;
        TeamTactics enemyTactics = isHomeAttacking ? awayTactics : homeTactics;
        List<MatchPlayer> enemies = isHomeAttacking ? awayTeam.Roster : homeTeam.Roster;

        // 최단 거리 0.03 미만 (interceptDist)인 적 탐색
        float closestDist = float.MaxValue;
        foreach (var e in enemies)
        {
            if (DistancePointToLineSegment(e.LogicPosition, passer.LogicPosition, receiver.LogicPosition) < interceptDist)
            {
                float distToPasser = CalculateDistance(passer.LogicPosition, e.LogicPosition);
                if (distToPasser < closestDist)
                {
                    closestDist = distToPasser;
                    pathEnemy = e;
                }
            }
        }

        if (pathEnemy == null)
        {
            Debug.Log($"<color=#00BFFF>[패스 디버그]</color> {passer.PlayerName} -> {receiver.PlayerName}\n" +
                      $"▶ 패스 경로에 방해하는 적이 없습니다. (100% 안전한 패스 성공)");
            return true;
        }

        float passStat = GetPlayerStat(passer, MatchStatType.Pass, homeTeam);
        float stealStat = GetPlayerStat(pathEnemy, MatchStatType.Steal, homeTeam);

        if (isHomeAttacking)
        {
            // 아군 공격 시: 패스 증가, 적 스틸 감소
            passStat += GetSynergyBonus(homeTeam, effectType.SystemBasket);
            stealStat = Mathf.Max(0, stealStat - GetSynergyBonus(homeTeam, effectType.AnkleBreaker));
        }

        passStat *= myTactics.bonusPass;
        stealStat *= enemyTactics.bonusSteal;

        // 기본 패스 성공 확률
        float prob = (passStat / (passStat + stealStat)) * 100f;
        float dice = Random.Range(0f, 100f);
        bool success = dice <= prob;

        string passerName = MakeName(passer.PlayerName);
        string receiverName = MakeName(receiver.PlayerName);
        string enemyName = MakeName(pathEnemy.PlayerName);

        Debug.Log($"<color=#00BFFF>[패스 디버그]</color> {passerName} -> {receiverName} (차단시도:{enemyName})\n" +
                  $"▶ 공격 패스스탯: {passStat:F2} | 수비 스틸스탯: {stealStat:F2} (차단판정거리: {interceptDist:F2})\n" +
                  $"▶ 공식: (({passStat:F2} / ({passStat:F2} + {stealStat:F2})) * 100) = {prob:F2}%\n" +
                  $"▶ <color=#00FF00>최종확률: {prob:F2}%</color> | 주사위: {dice:F2} => {(success ? "<b>패스 성공</b>" : "<b>차단당함!</b>")}");

        if (!success) interceptor = pathEnemy;
        return success;
    }

    // 드리블 성공 확률
    public static bool CalculateDribbleSuccess(MatchPlayer dribbler, MatchTeam homeTeam, MatchTeam awayTeam, TeamTactics homeTactics, TeamTactics awayTactics, float dribbleBlockDist)
    {
        bool isHomeAttacking = homeTeam.Roster.Contains(dribbler);
        TeamTactics myTactics = isHomeAttacking ? homeTactics : awayTactics;
        TeamTactics enemyTactics = isHomeAttacking ? awayTactics : homeTactics;
        List<MatchPlayer> enemies = isHomeAttacking ? awayTeam.Roster : homeTeam.Roster;

        MatchPlayer nearestEnemy = GetNearestPlayer(dribbler, enemies, out float minEnemyDist);

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

        float dribbleStat = GetPlayerStat(dribbler, MatchStatType.Pass, homeTeam);
        float stealStat = GetPlayerStat(nearestEnemy, MatchStatType.Steal, homeTeam);

        if (isHomeAttacking)
        {
            // 아군 드리블 시: 패스(드리블의 기반 스탯) 증가, 적 스틸 감소
            dribbleStat += GetSynergyBonus(homeTeam, effectType.SystemBasket);
            stealStat = Mathf.Max(0, stealStat - GetSynergyBonus(homeTeam, effectType.AnkleBreaker));
        }

        dribbleStat *= myTactics.bonusDribble;
        stealStat *= enemyTactics.bonusSteal;

        // 밸런스 가중치 적용
        float wStealBalance = MatchDataProxy.Instance.GetBalance("W_Def_Steal_Dribble");
        if (wStealBalance <= 0f) wStealBalance = 1.0f;

        float weightedStealStat = stealStat * wStealBalance;

        float prob = (dribbleStat / (dribbleStat + weightedStealStat)) * 100f;

        float dice = Random.Range(0f, 100f);
        bool success = dice <= prob;

        Debug.Log($"<color=#DA70D6>[드리블 디버그]</color> {dribblerName} 돌파 경합! (수비수: {enemyName}, 거리:{minEnemyDist:F2})\n" +
                  $"▶ 공격 드리블(패스)스탯: {dribbleStat:F2} | 수비 스틸스탯: {stealStat:F2} (가중치 {wStealBalance}배 적용 -> {weightedStealStat:F2})\n" +
                  $"▶ 공식: (({dribbleStat:F2} / ({dribbleStat:F2} + {weightedStealStat:F2})) * 100) = {prob:F2}%\n" +
                  $"▶ <color=#00FF00>최종 돌파 확률: {prob:F2}%</color> | 주사위: {dice:F2} => {(success ? "<b>돌파 성공(전진)!</b>" : "<b>수비에 막힘(좌우이동)</b>")}");

        return success;

    }

    // 리바운드 가중치 추첨
    public static MatchPlayer CalculateReboundWinner(Vector2 ballDropPos, List<MatchPlayer> allPlayers, MatchTeam homeTeam)
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
        float homeIronWall = GetSynergyBonus(homeTeam, effectType.IronWall);
        float totalTicket = 0f;
        List<float> tickets = new List<float>();

        for (int i = 0; i < candidates.Count; i++)
        {
            var p = candidates[i];
            float dist = CalculateDistance(p.LogicPosition, ballDropPos);

            float finalReboundStat = GetPlayerStat(p, MatchStatType.Rebound, homeTeam);

            // [시너지] 난공불락 스탯 부여
            if (homeTeam.Roster.Contains(p)) finalReboundStat += homeIronWall;

            float baseTicket = finalReboundStat * (1.0f - dist);

            float ticket = Mathf.Max(0.1f, baseTicket); // 티켓이 음수가 되지 않도록 최소 보장
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
