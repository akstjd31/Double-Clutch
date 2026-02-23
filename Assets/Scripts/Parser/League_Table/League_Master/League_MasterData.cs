using Google.GData.Documents;
using System;

public enum leagueType
{
    None,
    Tournament,
    Swiss
}
[Serializable]
public struct League_MasterData
{
    public string leagueId;
    public string desc;
    public string leagueNameKey;
    public leagueType leagueType;
    public int leagueTeamCount;
    public int roundCount;
    public int outConditionValue;
    public bool isSelectionRequired;
    public string teamSelectionRuleId;
    public string leagueLevelId;
    public string leagueRewardId;

    public League_MasterData(
        string leagueId, string desc, string leagueNameKey, 
        leagueType leagueType, int leagueTeamCount, int roundCount,
        int outConditionValue, bool isSelectionRequired, 
        string teamSelectionRuleId, string leagueLevelId, 
        string leagueRewardId)
    {
        this.leagueId = leagueId;
        this.desc = desc;
        this.leagueNameKey = leagueNameKey;
        this.leagueType = leagueType;
        this.leagueTeamCount = leagueTeamCount;
        this.roundCount = roundCount;
        this.outConditionValue = outConditionValue;
        this.isSelectionRequired = isSelectionRequired;
        this.teamSelectionRuleId = teamSelectionRuleId;
        this.leagueLevelId = leagueLevelId;
        this.leagueRewardId = leagueRewardId;
    }
}
