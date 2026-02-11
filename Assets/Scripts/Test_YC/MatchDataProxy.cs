using UnityEngine;

public static class MatchDataProxy
{
    public static float GetBalance(string key)
    {
        switch (key)
        {
            case "DIST_3POINT_LINE": return 0.35f; // 3점슛 라인 거리
            case "DIST_DUNK_RANGE": return 0.05f;  // 덩크 가능 거리
            case "DIST_DEFENSE_RANGE": return 0.05f; // 수비 인정 거리
            case "SHOOT_CONSTANT": return 1.0f;    // 슛 공식 상수
            case "DIST_PENALTY": return 0.5f;      // 거리 페널티 가중치
            default:
                Debug.LogError($"[MatchDataProxy] 알 수 없는 키값: {key}");
                return 0f;
        }

        // [나중] : 담당자가 코드를 주면 위 switch문을 지우고 아래 한 줄로
        // return Global.Data.MatchBalance.Get(key).Value; 
    }
}