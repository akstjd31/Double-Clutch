using System.Collections.Generic;
using UnityEngine;

public static class MatchCalculator
{

    public static bool CalculateShootSuccess(MatchPlayer shooter, MatchPlayer defender, float distanceToHoop)
    {
        // 필요한 수치를 "Proxy"에게 물어봅니다.
        float threePointLine = MatchDataProxy.GetBalance("DIST_3POINT_LINE");
        float penaltyWeight = MatchDataProxy.GetBalance("DIST_PENALTY");

        // 받아온 값으로 로직을 돌립니다.
        StatType shootStatType = (distanceToHoop > threePointLine) ? StatType.ThreePoint : StatType.TwoPoint;
        float shooterStat = shooter.GetStat(shootStatType);

        float blockerStat = 0f;
        if (defender != null)
        {
            blockerStat = defender.GetStat(StatType.Block);
        }

        // 공식 적용 (상수 대신 변수 사용)
        float distancePenalty = 1.0f + (distanceToHoop * penaltyWeight);

        float successProb = (shooterStat - (blockerStat * 0.5f)) / distancePenalty;
        successProb = Mathf.Clamp(successProb, 5.0f, 95.0f);

        float dice = Random.Range(0f, 100f);
        return dice <= successProb;
    }

    public static bool CalculatePassSuccess(MatchPlayer passer, MatchPlayer defender)
    {
        // 수비수가 없으면 무조건 성공
        if (defender == null) return true;

        float passStat = passer.GetStat(StatType.Pass);
        float stealStat = defender.GetStat(StatType.Steal);

        // 분모가 0이 되는 것을 방지
        float denominator = passStat + stealStat;
        if (denominator <= 0) denominator = 1f;

        float successProb = (passStat / denominator) * 100f;

        // 패스는 너무 자주 끊기면 게임이 루즈해지므로 최소 성공률 보정 (예: 20%)
        successProb = Mathf.Clamp(successProb, 20.0f, 99.0f);

        float dice = Random.Range(0f, 100f);
        return dice <= successProb;
    }

    public static MatchPlayer CalculateReboundWinner(List<MatchPlayer> nearbyPlayers)
    {
        if (nearbyPlayers == null || nearbyPlayers.Count == 0) return null;

        // 총 가중치 계산 (리바운드 스탯의 합)
        float totalWeight = 0f;
        foreach (var player in nearbyPlayers)
        {
            totalWeight += player.GetStat(StatType.Rebound);
        }

        // 랜덤 값 추출 (0 ~ 총합)
        float randomPoint = Random.Range(0f, totalWeight);

        // 당첨자 선정 (룰렛 방식)
        float currentWeight = 0f;
        foreach (var player in nearbyPlayers)
        {
            currentWeight += player.GetStat(StatType.Rebound);
            if (randomPoint <= currentWeight)
            {
                return player;
            }
        }

        // 혹시 계산 오류 시 첫 번째 선수 리턴
        return nearbyPlayers[0];
    }

    public static void SetRandomSeed(int seed)
    {
        Random.InitState(seed);
    }
}