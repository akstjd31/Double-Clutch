using System;
using System.Collections.Generic;

/// <summary>
/// 리그 전체 데이터
/// </summary>
[Serializable]
public class LeagueSaveData : SaveBase
{
    public string leagueId;
    public string leagueType;
    public int currentRoundIndex;
    public bool isFinished;
    public bool isPlayerEliminated;
    public List<string> teams = new();

    public LeagueSaveData() {}
}

/// <summary>
/// 참가 팀 정보
/// </summary>
[Serializable]
public class LeagueTeamEntry
{
    public string teamId;
    public bool isPlayerTeam;
    public bool isEliminated;
}

/// <summary>
/// 경기 기록 (이건 예찬님이 짠 코드로 대체 가능한지 물어봐야됨)
/// </summary>
[Serializable]
public class LeagueMatchRecord
{
    public int roundIndex;
    public string homeTeamId;
    public string awayTeamId;

    public bool isPlayed;
    public int homeScore;
    public int awayScore;

    public string specialNote;  // 몰수
    public string replayLogKey; // 리플레이 키
}

/// <summary>
/// 순위용 데이터
/// </summary>
[Serializable]
public class LeagueStandingData
{
    public string teamId;
    public int rank;
    public int played;
    public int win;
    public int lose;

    public int points;
    public int scored;
    public int conceded;
    public int goalDiff;
}