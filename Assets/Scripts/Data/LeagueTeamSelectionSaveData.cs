using System;
using System.Collections.Generic;

/// <summary>
/// 리그 전체 데이터
/// </summary>
[Serializable]
public class LeagueSaveData : SaveBase
{
    public string leagueId;                                 // 리그 ID
    public string leagueType;                               // 리그 타입(스위스 or 토너먼트)
    public int currentRoundIndex;                           // 현 라운드 정보
    public bool isFinished;                                 // 끝났는지에 대한 여부
    public bool isPlayerEliminated;                         // 우리 팀 탈락 여부
    public List<LeagueTeamEntry> teams = new();             // 참가 팀들
    public List<LeagueMatchRecord> matchRecords = new();    // 대진
    public List<LeagueStandingData> standings = new();      // 순위

    public LeagueSaveData() {}
}

/// <summary>
/// 참가 팀 정보
/// </summary>
[Serializable]
public class LeagueTeamEntry : SaveBase
{
    public string teamId;
    public bool isPlayerTeam;
    public bool isEliminated;
}

/// <summary>
/// 경기 기록
/// </summary>
[Serializable]
public class LeagueMatchRecord : SaveBase
{
    public int roundIndex;      // 라운드 번호
    public string homeTeamId;   // 우리 ID
    public string awayTeamId;   // 상대 ID

    public bool isPlayed;       // 플레이중인지?
    public int homeScore;       // 우리팀
    public int awayScore;       // 상대팀

    public string specialNote;  // 몰수
    public string replayLogKey; // 리플레이 키
}

/// <summary>
/// 순위용 데이터
/// </summary>
[Serializable]
public class LeagueStandingData : SaveBase
{
    public string teamId;   // 팀 ID
    public int rank;        // 순위
    public int played;      // 진행한 라운드 수(?)
    public int win;         // 이긴 횟수
    public int lose;        // 진 횟수

    public int points;      // 포인트
    public int scored;      // 우리 점수
    public int conceded;    // 상대 점수
    public int goalDiff;    // 차이
}