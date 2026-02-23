using Google.GData.Documents;
using System;

public enum positionType
{
    None,PG,SG,SF,PF,C
}
public enum handicapState
{
    None
}
[Serializable]
public struct Mercenary_Data
{
    public int playerUId;
    public positionType positionType;
    public string mercName;
    public int traitId;
    public int passiveId1;
    public int passiveId2;
    public int passiveId3;
    public int stat2ptValue;
    public int stat3ptValue;
    public int statAssistValue;
    public int statBlockValue;
    public int statStealValue;
    public int statReboundValue;
    public int currentCondition;
    public handicapState handicapState;
    public string playerImageResource;
    public string portraitResource;

    public Mercenary_Data(
        int playerUId,
        positionType positionType,
        string mercName,
        int traitId,
        int passiveId1,
        int passiveId2,
        int passiveId3,
        int stat2ptValue,
        int stat3ptValue,
        int statAssistValue,
        int statBlockValue,
        int statStealValue,
        int statReboundValue,
        int currentCondition,
        handicapState handicapState,
        string playerImageResource,
        string portraitResource
    )
    {
        this.playerUId = playerUId;
        this.positionType = positionType;
        this.mercName = mercName;

        this.traitId = traitId;

        this.passiveId1 = passiveId1;
        this.passiveId2 = passiveId2;
        this.passiveId3 = passiveId3;

        this.stat2ptValue = stat2ptValue;
        this.stat3ptValue = stat3ptValue;
        this.statAssistValue = statAssistValue;
        this.statBlockValue = statBlockValue;
        this.statStealValue = statStealValue;
        this.statReboundValue = statReboundValue;

        this.currentCondition = currentCondition;

        this.handicapState = handicapState;

        this.playerImageResource = playerImageResource;
        this.portraitResource = portraitResource;
    }
}
