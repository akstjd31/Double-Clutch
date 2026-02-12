using UnityEngine;

// CSV의 String 데이터와 매핑될 Enum 정의
// 0부터 시작
public enum Position
{
    PG = 0, // Point Guard
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
    Dribble,       // 드리블 (기획서 1.2)
    Block,         // 블록
    Steal,         // 스틸
    Rebound,       // 리바운드
    Speed,         // 이동 속도 (시뮬레이션 연산용)
    Stamina        // 체력
}
