using System;
using System.Data.SqlTypes;

[Serializable]
public class PlayerSaveData : SaveBase
{
    public string schoolName;
    public string coachName;    
    public int honor;           // 명성
    public int money;
    public int weekId;          // 테이블 주 ID
}