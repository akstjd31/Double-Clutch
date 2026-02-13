using System;
[Serializable]
public struct League_TeamData
{
    public string teamSelectionRuleId;
    public string desc;
    public int weekId;
    public int priorityTeamCount;
    public int selectionCountTotal;
    public int leagueTeamTotal;
    public int selectionCountD;
    public int selectionCountC;
    public int selectionCountB;
    public int selectionCountA;
    public int selectionCountS;
    public int selectionCountSS;
    public int selectionCountSSS;

    public League_TeamData(string teamSelectionRuleId,
    string desc,
    int weekId,
    int priorityTeamCount,
    int selectionCountTotal,
    int leagueTeamTotal,
    int selectionCountD,
    int selectionCountC,
    int selectionCountB,
    int selectionCountA,
    int selectionCountS,
    int selectionCountSS,
    int selectionCountSSS)
    {
        this.teamSelectionRuleId = teamSelectionRuleId;
        this.desc = desc;
        this.weekId = weekId;
        this.priorityTeamCount = priorityTeamCount;
        this.selectionCountTotal = selectionCountTotal;
        this.leagueTeamTotal = leagueTeamTotal;
        this.selectionCountD = selectionCountD;
        this.selectionCountC = selectionCountC;
        this.selectionCountB = selectionCountB;
        this.selectionCountA = selectionCountA;
        this.selectionCountS = selectionCountS;
        this.selectionCountSS = selectionCountSS;
        this.selectionCountSSS = selectionCountSSS;
    }
}
