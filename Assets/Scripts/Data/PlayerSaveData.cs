using System;
using System.Collections.Generic;


[Serializable]
public class PlayerSaveData : SaveBase
{
    public string schoolName;
    public string coachName;    
    public int honor;           // 명성
    public int money;
    public int weekId;          // 테이블 주 ID
    public int year;            // 연차
    public int totalWinHonor;   // 리그 우승으로 누적된 명성치 (졸업 시즌에 받음)
    public bool[] tutorialCompleted;    // 튜토리얼 완료 여부
    public string currentProfileImage;
    public bool isGraduationPending;
    public List<GraduationRecord> graduationRecord = new List<GraduationRecord>();
    public List<LeagueWinRecord> leagueWinRecord = new List<LeagueWinRecord>();
}

/// <summary>
/// 1년 동안 각 리그별 우승했는지 여부 확인용 클래스
/// </summary>
[Serializable]
public class LeagueWinRecord
{
    public string leagueId;     // 리그 이름 or ID
    public bool hasWon;         // 우승 여부

    public LeagueWinRecord(string leagueId, bool hasWon)
    {
        this.leagueId = leagueId;
        this.hasWon = hasWon;
    }
}