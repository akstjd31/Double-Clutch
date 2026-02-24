using System;
using System.Data.SqlTypes;

[Serializable]
public class PlayerSaveData : SaveBase
{
    public string schoolName;
    public string coachName;
    public int money;
    public int weekId;
}