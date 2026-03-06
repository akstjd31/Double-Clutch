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
    public int grade;
    public effectType effectType;
    public float effectValue;
    public string passiveDesc;

    public Player_PassiveData
        (
            string _skillId, string _skillName, int _grade,
            effectType _effectType, float _effectValue,
            string _passiveDesc
        )
    {
        skillId = _skillId;
        skillName = _skillName;
        grade = _grade;
        effectType = _effectType;
        effectValue = _effectValue;
        passiveDesc = _passiveDesc;
    }
}
