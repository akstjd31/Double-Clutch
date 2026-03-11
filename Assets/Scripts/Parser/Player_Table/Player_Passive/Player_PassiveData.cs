using System;

public enum skillCategory
{
    None, Match, Training
}
public enum effectType
{    
    None,
    TransitionMaster,  // 트랜지션 마스터 (2점 슛 점수 로직 증가)
    GorillaDunk,       // 고릴라 덩크 (골대 근처 슛 성공률 증가)
    IronWall,          // 난공불락 (아군 리바운드 능력치 증가)
    CutInPlay,         // 컷인 플레이 (2점슛 성공률 증가)
    SpaceOperator,     // 스페이스 오퍼레이터 (3점슛 성공률 증가)
    HighlightFilm,     // 하이라이트 필름 (골대로 돌진하여 덩크슛)
    SystemBasket,      // 시스템 바스켓 (아군 패스 능력치 증가)
    AnkleBreaker,      // 앵클 브레이커 (상대 스틸 능력치 감소)
    SuffocatingDefense,// 질식 수비 (아군 블락 능력치 증가)
    ClutchHeart,       // 승부사의 심장 (10점차 지고 있을 때 3점슛 확률 증가)
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
