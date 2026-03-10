using System;

public enum skillCategory
{
    None, Match, Training
}
public enum effectType
{    
    None, 

    Prob2pt, Prob3pt, ProbDunk, ProbSteal, GrowthShoot, //매치 수정 후 이 줄 날리기!

    
    Rate2pt, Rate3pt, RateSteal, RateBlock, RatePass, RateRebound, //해당 스탯을 %로 상승
    Poten2pt, Poten3pt, PotenSteal, PotenBlock, PotenPass, PotenRebound, //해당 스탯 최대잠재력을 고정값 상승
    Growth2pt, Growth3pt, GrowthSteal, GrowthBlock, GrowthPass, GrowthRebound, //해당 스탯 증가 훈련 시 상승치 고정값 증가
    GoldUp, ReputationUp, GraduationGold //골드 %상승, 졸업시 획득 명성 고정값, 졸업시 골드 획득 고정값    
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
