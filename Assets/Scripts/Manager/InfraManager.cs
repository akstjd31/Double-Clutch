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
            MaxLevelData.Add(data.infraEffectType, SetMaxLevelByGroup(data.infraEffectType));
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
    public int SetMaxLevelByGroup(infraEffectType infraET)
    {
        if (_reader == null) return -1;
        if (infraET.Equals(infraEffectType.None)) return -1;

        int maxLv = 0;
        foreach (Infra_Data data in _reader.DataList)
        {
            if (infraET.Equals(data.infraEffectType)) continue;
            if (maxLv < data.infraLevel) maxLv = data.infraLevel;
        }

        return maxLv;
    }
}
