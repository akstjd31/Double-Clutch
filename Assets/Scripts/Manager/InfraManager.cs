using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using System.Collections.Generic;

public class InfraManager : Singleton<InfraManager>
{
    [SerializeField] private Infra_DataReader _reader;
    public SerializedDictionary<infraEffectType, int> MaxLevelData;  // 그룹 Id, 최대 레벨

    protected override void Awake()
    {
        base.Awake();
        MaxLevelData = new SerializedDictionary<infraEffectType, int>();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (Infra_Data data in _reader.DataList)
        {
            if (MaxLevelData.ContainsKey(data.infraEffectType)) continue;
            MaxLevelData.Add(data.infraEffectType, GetMaxLevelByEffectType(data.infraEffectType));
        }
    }

    // public int GetCostByEffectTypeAndLevel(infraEffectType infraET, int level)
    // {
    //     if (_reader == null) return -1;
        
    //     foreach (Infra_Data data in _reader.DataList)
    //     {
    //         if (infraET.Equals(data.infraEffectType))
    //         {
                
    //         }
    //     }
    // }

    // 그룹 ID 중 맥스 레벨 반환
    public int GetMaxLevelByEffectType(infraEffectType infraET)
    {
        if (_reader == null) return -1;
        if (infraET.Equals(infraEffectType.None)) return -1;

        int maxLv = 0;
        foreach (var data in _reader.DataList)
        {
            if (!infraET.Equals(data.infraEffectType)) continue;
            if (maxLv < data.infraLevel) maxLv = data.infraLevel;
        }

        return maxLv;
    }

    // 같은 타입 중에 비용 관련된 리스트 반환
    public List<int> GetCostListByEffectType(infraEffectType infraET)
    {
        if (_reader == null) return null;
        if (infraET.Equals(infraEffectType.None)) return null;

        var costList = new List<int>();
        foreach (var data in _reader.DataList)
        {
            if (!infraET.Equals(data.infraEffectType)) continue;
            costList.Add(data.infraCost);

        }

        return costList;
    }

    // 해당 데이터 반환 (구조체라 Nullable로 바꿔줌)
    public Infra_Data? GetDataByEffectType(infraEffectType infraET)
    {
        if (_reader == null) return null;
        if (infraET.Equals(infraEffectType.None)) return null;

        foreach (var data in _reader.DataList)
        {
            if (!infraET.Equals(data.infraEffectType)) continue;
            return data;
        }

        return null;
    }
}
