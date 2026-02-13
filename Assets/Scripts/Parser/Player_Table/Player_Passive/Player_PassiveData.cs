using System;


[Serializable]
public struct Player_PassiveData
{
    public int skillId;
    public string skillName;
    public skillCategory skillCategory;
    public string triggerType;
    public int triggerValue;
    //public effectType effectType;
    public float effectValue;
    public int effectDuration;
    public int CoolTime;
    public string passiveDesc;

    public Player_PassiveData
        (
            int _skillId, string _skillName, skillCategory _skillCategory,
            string _triggerType, int _triggerValue, float _effectValue, int _effectDuration,
            int _CoolTime, string _passiveDesc
        )
    {
        skillId = _skillId;
        skillName = _skillName;
        skillCategory = _skillCategory;
        triggerType = _triggerType;
        triggerValue = _triggerValue;
        effectValue = _effectValue;
        effectDuration = _effectDuration;
        CoolTime = _CoolTime;
        passiveDesc = _passiveDesc;
    }
}
