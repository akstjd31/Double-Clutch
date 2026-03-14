using System;
using System.Collections.Generic;

[Serializable]
public class LeagueResultSaveData : SaveBase
{
    public string leagueId;
    public List<string> teams = new();

    public LeagueResultSaveData() {}
}


[Serializable]
public class LeagueResultTeamData : SaveBase
{
    public string teamId;
    public int rank;

    public LeagueResultTeamData() {}
}
