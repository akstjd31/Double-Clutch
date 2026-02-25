using System;

public enum infraEffectType
{
    None, 
    TrainingBonus, 
    RestCostDiscount,
    RewardGoldBonus,
    AddTactic,
    AddRoster
}

[Serializable]
public struct Infra_Data
{
    public int index;
    public int group;
    public string desc;
    public int infraLevel;
    public infraEffectType infraEffectType;
    public int infraEffectValue;
    public int infraCost;
    public string infraNameKey;
    public string infraDescKey;
    public string icon;

    public Infra_Data(int idx, int gp, string desc, int infraLv, infraEffectType infraET, int infraEV, int infraCost, string infraNK, string infraDK, string icon)
    {
        index = idx;
        group = gp;
        this.desc = desc;
        infraLevel = infraLv;
        infraEffectType = infraET;
        infraEffectValue = infraEV;
        this.infraCost = infraCost;
        infraNameKey = infraNK;
        infraDescKey = infraDK;
        this.icon = icon;
    }
}
