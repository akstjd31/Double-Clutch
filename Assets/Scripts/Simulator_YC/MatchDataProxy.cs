using UnityEngine;
using System.Collections.Generic;

// 전술 데이터 구조체 (가중치 저장용)
public struct TeamTactics
{
    public float w2PT;      // 2점슛 선호도
    public float w3PT;      // 3점슛 선호도
    public float wPass;     // 패스 선호도
    public float wDribble;  // 드리블 선호도 (테이블에 없어서 기본값 1.0 사용)

    public TeamTactics(float w2, float w3, float wp)
    {
        w2PT = w2;
        w3PT = w3;
        wPass = wp;
        wDribble = 1.0f; // 기본값
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
        // CSV 데이터 하드코딩 (나중에 파서로 교체될 부분)
        switch (teamColorId)
        {
            // [예외] 테이블에서 유일하게 가중치가 다른 전술
            case "TC_SHT_Sniper": // 3점슛 특화 듀얼 가드 (w3PT = 1.5)
                return new TeamTactics(1.0f, 1.5f, 1.0f);

            // [공통] 나머지 모든 전술은 테이블상 가중치가 전부 1.0임
            case "TC_SML_Base":
            case "TC_BAL_Base":
            case "TC_OFF_Base":
            case "TC_SHT_Base": 
            case "TC_DEF_Base":
            case "TC_TAC_Base":
            case "TC_BIG_Base":
            default:
                return new TeamTactics(1.0f, 1.0f, 1.0f);
        }
    }
}
