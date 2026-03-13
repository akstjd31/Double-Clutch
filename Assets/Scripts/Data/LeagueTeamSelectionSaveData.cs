using System;
using System.Collections.Generic;

[Serializable]
public class LeagueTeamSelectionSaveData : SaveBase
{
    public string teamSelectionRuleId;     
    public int selectedWeekId;          
    public string entryKey;
    public List<string> selectedTeamIds = new List<string>();
}

public enum TeamSelectionAction
{
    None,
    ReuseCache,
    RunSelection,
    Error
}