using System;

[Serializable]
public struct Calendar_TableData
{
    public int weekid;
    public string desc;
    public int month;
    public int weekNo;
    public phaseType phase;
    public bool isSpecialWeek;
    public bool hasSeasonOut;
    public int targetidDefault;
    public int targetidSpecial;
    public string leagueId;
    public string startCutscene;
    public string endCutscene;
    public string TutorialId;
    public string backgroundImageId;
    public string backgroundMusicId;

    public Calendar_TableData(
        int weekid, string desc, int month, int weekNo, phaseType phase,
        bool isSpecialWeek, bool hasSeasonOut, int targetidDefault, int targetidSpecial,
        string leagueId, string startCutscene, string endCutscene, string tutorialId,
        string backgroundImageId, string backgroundMusicId)
    {
        this.weekid = weekid;
        this.desc = desc;
        this.month = month;
        this.weekNo = weekNo;
        this.phase = phase;
        this.isSpecialWeek = isSpecialWeek;
        this.hasSeasonOut = hasSeasonOut;
        this.targetidDefault = targetidDefault;
        this.targetidSpecial = targetidSpecial;
        this.leagueId = leagueId;
        this.startCutscene = startCutscene;
        this.endCutscene = endCutscene;
        this.TutorialId = tutorialId;
        this.backgroundImageId = backgroundImageId;
        this.backgroundMusicId = backgroundMusicId;
    }
}
