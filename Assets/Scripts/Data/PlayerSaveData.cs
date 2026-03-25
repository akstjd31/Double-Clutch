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
    public bool isTutorialCompleted;    // 튜토리얼 완료 여부

    public string currentProfileImage;

    public List<GraduationRecord> graduationRecord = new List<GraduationRecord>();

    public bool isGraduationPending;

}