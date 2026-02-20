using UnityEngine;
using System.Collections.Generic;

// 전술 데이터 구조체 (스탯 보정값 저장용)
public struct TeamTactics
{
    public float bonusTwoPoint;   // 2점슛 스탯 보정
    public float bonusThreePoint; // 3점슛 스탯 보정
    public float bonusPass;       // 패스 스탯 보정
    public float bonusBlock;      // 블록 스탯 보정
    public float bonusSteal;      // 스틸 스탯 보정
    public float bonusRebound;    // 리바운드 스탯 보정
    public float bonusDribble;    // 드리블 스탯 보정

    // 테이블 미정 → 일단 전부 1.0 (보정 없음)
    public TeamTactics(float tp = 1.0f, float three = 1.0f, float pass = 1.0f,
                       float block = 1.0f, float steal = 1.0f, float rebound = 1.0f,
                       float dribble = 1.0f)
    {
        bonusTwoPoint = tp;
        bonusThreePoint = three;
        bonusPass = pass;
        bonusBlock = block;
        bonusSteal = steal;
        bonusRebound = rebound;
        bonusDribble = dribble;
    }
}


public static class MatchDataProxy
{
    public static float GetBalance(string key)
    {
        switch (key)
        {
            case "DIST_3POINT_LINE": return 0.35f;
            case "DIST_DUNK_RANGE": return 0.05f;
            case "DIST_PENALTY": return 0.5f;
            default:
                Debug.LogError($"[MatchDataProxy] 알 수 없는 키값: {key}");
                return 0f;
        }

        // [나중] : 담당자가 코드를 주면 위 switch문을 지우고 아래 한 줄로
        // return Global.Data.MatchBalance.Get(key).Value; 
    }

    // 전술 ID로 가중치 가져오기
    // (League_Table.xlsx - Team_Color_Table.csv 데이터 기반)
    public static TeamTactics GetTactics(string teamColorId)
    {
        // 테이블 미정 → 전부 기본값 1.0으로 반환
        // 추후 테이블 확정되면 여기에 전술별 보정값 채워넣기
        switch (teamColorId)
        {
            case "TC_DEF_Base":
            case "TC_OFF_Base":
            case "TC_BAL_Base":
            case "TC_SHT_Base":
            case "TC_SHT_Sniper":
            case "TC_TAC_Base":
            case "TC_BIG_Base":
            case "TC_SML_Base":
            default:
                return new TeamTactics(); // 전부 1.0
        }
    }

}
