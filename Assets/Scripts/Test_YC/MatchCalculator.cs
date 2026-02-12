using System.Collections.Generic;
using UnityEngine;

public static class MatchCalculator
{

    public static bool CalculateShootSuccess(MatchPlayer attacker, MatchPlayer defender, float distanceToHoop)
    {
        // 기준 데이터 가져오기
        float threePointLine = MatchDataProxy.GetBalance("DIST_3POINT_LINE"); // 0.35f
        float dunkRange = MatchDataProxy.GetBalance("DIST_DUNK_RANGE");       // 0.05f
        float penaltyWeight = MatchDataProxy.GetBalance("DIST_PENALTY");

        // 슛 타입 결정 (스탯 선택)
        StatType shootStatType;

        if (distanceToHoop >= threePointLine)
        {
            shootStatType = StatType.ThreePoint; // 3점슛 스탯
        }
        else
        {
            shootStatType = StatType.TwoPoint;  // 2점슛 스탯 (덩크 포함)
        }

        float shooterStat = attacker.GetStat(shootStatType);

        // 수비 방해 계산
        float blockerStat = 0f;
        if (defender != null)
        {
            blockerStat = defender.GetStat(StatType.Block);
        }

        // 거리 페널티 적용 (기획서: 거리가 멀수록 분모에 값을 더해 확률을 낮춤)
        // 덩크(0.05 이하)는 거리 페널티를 받지 않도록 1.0 고정 (혹은 기획 의도에 따라 이것도 페널티 줄 수 있음)
        float distancePenalty = 1.0f;
        if (distanceToHoop > dunkRange)
        {
            distancePenalty = 1.0f + (distanceToHoop * penaltyWeight);
        }

        // 최종 확률 계산
        float successProb = (shooterStat - (blockerStat * 0.5f)) / distancePenalty;

        // 덩크 슛 보정확률 넣을지 묻기

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

        successProb = Mathf.Clamp(successProb, 0.0f, 100.0f);

        float dice = Random.Range(0f, 100f);
        return dice <= successProb;
    }
    public static bool CalculateDribbleSuccess(MatchPlayer attacker, MatchPlayer defender)
    {
        // 수비수가 없으면 프리패스
        if (defender == null) return true;

        // 능력치 가져오기
        float dribbleStat = attacker.GetStat(StatType.Dribble);

        // 수비 스탯: 스틸과 블록의 평균, 혹은 'Speed'를 수비력으로 쓸 수도 있음 (기획 의도에 따라 조정)
        // 여기서는 수비수의 'Steal' 능력이 드리블을 막는 주 스탯이라고 가정
        float defenseStat = defender.GetStat(StatType.Steal);

        // 확률 계산 ( 현재 드리블 공식 명시되지않아서 임의로 패스 공식 차용 )
        float denominator = dribbleStat + defenseStat;
        if (denominator <= 0) denominator = 1f;

        float successProb = (dribbleStat / denominator) * 100f;

        successProb = Mathf.Clamp(successProb, 10.0f, 90.0f);

        // 주사위 굴리기
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
