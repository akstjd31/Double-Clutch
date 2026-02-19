using System;

public enum triggerCond
{
    None, Random, ReboundDiff, Stat2ptLow, Stat3ptLow, ScoreGap
}
[Serializable]

public struct Halftime_ListData
{
    public int scriptId;
    public string desc;
    public triggerCond triggerCond;
    public int triggerValue;

    public Halftime_ListData(int scriptId, string desc, triggerCond triggerCond, int triggerValue)
    {
        this.scriptId = scriptId;
        this.desc = desc;
        this.triggerCond = triggerCond;
        this.triggerValue = triggerValue;
    }
}
