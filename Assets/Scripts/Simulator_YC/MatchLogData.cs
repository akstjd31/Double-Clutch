using UnityEngine;

[System.Serializable]
public class MatchLogData
{
    public float GameTime;      // 남은 시간
    public int Quarter;         // 쿼터
    public string LogText;      // 로그 텍스트
    public int TeamId;          // 0: Home, 1: Away
    public int PlayerId;        // 선수 ID
    public string PlayerName;   // 선수 이름

    // 이벤트 타입: "QUARTER_START", "MOVE", "SHOOT", "GOAL", "MISS", "PASS", "STEAL", "DRIBBLE", "BLOCK", "REBOUND", "MATCH_END"
    public string EventType;

    public bool IsSuccess;      // 성공 여부
    public int ScoreAdded;      // 득점 (2 or 3)

    // 연출용 데이터
    public bool IsCutIn;        // 컷인 연출 여부
    public string CutInType;    // "DUNK", "3PT", "BUZZER"

    public Vector2 BallPos;     // 공이 위치해야 할 논리 좌표 (0~1)

    // 오프볼 무브 연출을 위해 10명 선수의 좌표를 매 턴 저장
    public Vector2[] HomePositions = new Vector2[5];
    public Vector2[] AwayPositions = new Vector2[5];
    // 생성자 없이 기본 생성자 사용
    public MatchLogData() { }
}
