using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using System.Collections.Generic;

public class InfraManager : Singleton<InfraManager>
{
    [SerializeField] private Infra_DataReader _reader;
    [SerializeField] private SerializedDictionary<infraEffectType, string> _infraDescData; // 시설 효과 설명 관련 딕셔너리

    protected override void Awake()
    {
        base.Awake();
        _infraDescData = new SerializedDictionary<infraEffectType, string>();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_reader == null) return;

        var stringMgr = StringManager.Instance;
        if (stringMgr == null) return;

        foreach (var data in _reader.DataList)
        {
            if (_infraDescData.ContainsKey(data.infraEffectType)) continue;
            string desc = stringMgr.GetString(data.infraDescKey);
            _infraDescData[data.infraEffectType] = desc;

            // var valueStr = TextParser.GetKeys(desc);
            //FormatInfraDescText(desc, valueStr[0], data.infraEffectValue);
        }
    }

    // 중괄호로 되어있는 부분을 처리 및 전체 문자열을 반환
    // private string FormatInfraDescText(string desc, string target, int infraEffectValue) => desc.Replace(target, infraEffectValue.ToString());

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

    public string GetInfraDescByEffectType(infraEffectType infraET) => _infraDescData[infraET];
}
