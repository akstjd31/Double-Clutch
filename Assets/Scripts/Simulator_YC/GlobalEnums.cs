using UnityEngine;

// CSV의 String 데이터와 매핑될 Enum 정의
// 0부터 시작
public enum Position
{
    None = 0, 
    PG,     // Point Guard
    SG,     // Shooting Guard
    SF,     // Small Forward
    PF,     // Power Forward
    C       // Center
}

public enum TeamSide
{
    Home = 0, // 유저 팀
    Away      // 상대 팀 (AI)
}

public enum MatchStatType
{
    TwoPoint = 0,  // 2점 슛
    ThreePoint,    // 3점 슛
    Pass,          // 패스
    Block,         // 블록
    Steal,         // 스틸
    Rebound,       // 리바운드
}
