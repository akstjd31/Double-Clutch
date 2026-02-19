using System;
[Serializable]
public struct League_TeamData
{
    public string teamSelectionRuleId;
    public string desc;
    public int weekId;
    public int priorityTeamCount;
    public string prioritySourceLeagueId;
    public string candidateSectorList;
    public int candidateSectorMask;
    public int leagueTeamTotal;
    public int selectionCountTotal;
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
    string prioritySourceLeagueId,
    string candidateSectorList,
    int candidateSectorMask,
    int leagueTeamTotal,
    int selectionCountTotal,
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
        this.prioritySourceLeagueId = prioritySourceLeagueId;
        this.candidateSectorList = candidateSectorList;
        this.candidateSectorMask = candidateSectorMask;
        this.leagueTeamTotal = leagueTeamTotal;
        this.selectionCountTotal = selectionCountTotal;
        this.selectionCountD = selectionCountD;
        this.selectionCountC = selectionCountC;
        this.selectionCountB = selectionCountB;
        this.selectionCountA = selectionCountA;
        this.selectionCountS = selectionCountS;
        this.selectionCountSS = selectionCountSS;
        this.selectionCountSSS = selectionCountSSS;
    }
}
