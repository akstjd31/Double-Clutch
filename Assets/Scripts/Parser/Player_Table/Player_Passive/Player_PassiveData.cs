using System;

public enum skillCategory
{
    None, Match, Training
}
public enum effectType
{
    //    2ptProb, 3ptProb, 
    None, Prob2pt, Prob3pt, ProbDunk, ProbSteal, GrowthShoot, GrowthSteal
}

[Serializable]
public struct Player_PassiveData
{
    public string skillId;
    public string skillName;
    public skillCategory skillCategory;
    public triggerCond triggerCond;
    public int triggerValue;
    public effectType effectType;
    public float effectValue;
    public int effectDuration;
    public int coolTime;
    public string passiveDesc;

    public Player_PassiveData
        (
            string _skillId, string _skillName, skillCategory _skillCategory,
            triggerCond _triggerCond, int _triggerValue, effectType _effectType, float _effectValue, int _effectDuration,
            int _CoolTime, string _passiveDesc
        )
    {
        skillId = _skillId;
        skillName = _skillName;
        skillCategory = _skillCategory;
        triggerCond = _triggerCond;
        triggerValue = _triggerValue;
        effectType = _effectType;
        effectValue = _effectValue;
        effectDuration = _effectDuration;
        coolTime = _CoolTime;
        passiveDesc = _passiveDesc;
    }
}
