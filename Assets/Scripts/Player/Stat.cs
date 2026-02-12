using System;
using UnityEngine;

public enum StatType
{
    Shot, // 2점 슛 능력
    ThreePoint, // 3점 슛 능력
    Assist, // 어시스트 능력
    Block, // 블락 능력
    Steal, // 스틸 능력
    Rebound // 리바운드 능력
}

[Serializable] //저장 가능하도록 직렬화
public class Stat
{
    [SerializeField] StatType _type;//스탯의 종류
    [SerializeField] int _current { get; set; } //현재 스탯값
    [SerializeField] int _limit { get; set; } //스탯 최대 성장치


    public StatType Type => _type;
    public int Current => _current;
    public int Limit => _limit;


    public Stat(StatType type, int current, int limit) //생성자에서 종류, 초기값, 최대치 지정
    {
        _type = type;
        _current = current;
        _limit = limit;
    }

    public int GrowAndReturn(int amount) // 스탯을 성장시키고 성장치를 반환하는 메서드
    {
        int before = Current;

        _current = Mathf.Clamp(Current + amount, 0, Limit);

        int growth = Current - before;

        return growth;
    }
}