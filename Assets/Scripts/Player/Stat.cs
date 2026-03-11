using System;
using UnityEngine;

public enum potential
{
    None,
    Stat2pt, // 2점 슛 능력
    Stat3pt, // 3점 슛 능력
    StatPass, // 어시스트 능력    
    StatSteal, // 스틸 능력
    StatBlock, // 블락 능력
    StatRebound // 리바운드 능력
}

[Serializable] //저장 가능하도록 직렬화
public class Stat
{
    [SerializeField] potential _type;//스탯의 종류
    [SerializeField] int _current; //현재 스탯값
    [SerializeField] int _limit; //스탯 최대 성장치
    [SerializeField] int _growthRate; //스탯 성장률
    [SerializeField] int _finalCurrent; //보너스 적용 현재 스탯값
    
    float _statBonusPercent = 0;
    float _limitBonus = 0;

    public potential Type => _type;
    public int Current => _finalCurrent;
    public int Limit => _limit + (int)_limitBonus;
    public int GrowthRate => _growthRate;
    public int InfraBonus => InfraManager.Instance.GetInfraEffectValueByEffectType(infraEffectType.AddTactic);
    public int RawCurrent => _current;
    public int RawLimit => _limit;

    public Stat(potential type, int current, int limit, int growthRate) //생성자에서 종류, 초기값, 최대치 지정
    {
        _type = type;
        _current = current;
        _limit = limit;
        _growthRate = growthRate;
    }

    public int GrowAndReturn(int amount) // 스탯을 성장시키고 성장치를 반환하는 메서드
    {
        int before = _current;

        _current = Mathf.Clamp(_current + amount, 0, Limit);

        int growth = _current - before;

        RefreshFinalStat();

        return growth;
    }

    public void RefreshFinalStat() 
    {
        //Debug.Log("스탯 재계산!");
        //Debug.Log($"{_type} : 기본 {_current}| 인프라 {InfraBonus}| 패시브 스탯{_statBonusPercent} | 패시브 리밋 {_limitBonus} | 최종 {_finalCurrent}");
        _finalCurrent = (int)((_current + InfraBonus ) * (1f + _statBonusPercent));         
    }

    public void SetPassiveBonus(float rate, float poten)
    {
        _statBonusPercent = rate;
        _limitBonus = poten;        
    }
}